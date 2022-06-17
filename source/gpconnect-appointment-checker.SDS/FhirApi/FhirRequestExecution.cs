using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class FhirRequestExecution : IFhirRequestExecution
    {
        private static ILogger<FhirRequestExecution> _logger;
        private static ILogService _logService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsMonitor<Spine> _spineOptionsDelegate;

        public FhirRequestExecution(ILogger<FhirRequestExecution> logger, ILogService logService, IHttpClientFactory httpClientFactory, IOptionsMonitor<Spine> spineOptionsDelegate)
        {
            _logger = logger;
            _logService = logService;
            _httpClientFactory = httpClientFactory;
            _spineOptionsDelegate = spineOptionsDelegate;
        }

        public async Task<T> ExecuteFhirQuery<T>(string query, string baseAddress, CancellationToken cancellationToken, SpineMessageTypes spineMessageType) where T : class
        {
            var getRequest = new HttpRequestMessage();
            var spineMessage = new SpineMessage()
            {
                SpineMessageTypeId = (int)spineMessageType
            };
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var client = _httpClientFactory.CreateClient("GpConnectClient");
                client.DefaultRequestHeaders.Add(Constants.Headers.ApiKey, _spineOptionsDelegate.CurrentValue.SpineFhirApiKey);

                getRequest.Method = HttpMethod.Get;
                getRequest.RequestUri = GenerateUri(baseAddress, query);

                var response = client.Send(getRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var contents = await response.Content.ReadAsStringAsync(cancellationToken);

                spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                spineMessage.ResponsePayload = contents;
                spineMessage.ResponseStatus = response.StatusCode.ToString();
                spineMessage.RequestPayload = getRequest.ToString();
                spineMessage.ResponseHeaders = response.Headers.ToString();

                sw.Stop();
                spineMessage.RoundTripTimeMs = sw.Elapsed.TotalMilliseconds;
                var savedSpineMessage = _logService.AddSpineMessageLog(spineMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseContents = JsonConvert.DeserializeObject<T>(contents);
                    return responseContents;
                }
                return null;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An Exception has occurred while attempting to execute a FHIR API query");
                throw;
            }
        }

        private Uri GenerateUri(string baseAddress, string query)
        {
            baseAddress = AddScheme(baseAddress);
            baseAddress = AddQuery(baseAddress, query);
            var uri = new Uri(baseAddress);
            return uri;
        }

        private string AddQuery(string baseAddress, string query)
        {
            baseAddress = baseAddress.EndsWith("/") ? baseAddress.Remove(baseAddress.Length - 1) : baseAddress;
            query = query.StartsWith("/") ? query : "/" + query;
            return baseAddress + query;
        }

        private string AddScheme(string baseAddress)
        {
            return !baseAddress.StartsWith("https://") ? "https://" + baseAddress : baseAddress;
        }
    }
}
