using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GpConnect.AppointmentChecker.Api.Service.Fhir;

public class FhirRequestExecution : IFhirRequestExecution
{
    private static ILogger<FhirRequestExecution> _logger;
    private static ILogService _logService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;

    public FhirRequestExecution(ILogger<FhirRequestExecution> logger, ILogService logService, IHttpClientFactory httpClientFactory, IOptions<SpineConfig> spineOptionsDelegate)
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
            client.DefaultRequestHeaders.Add(Headers.ApiKey, _spineOptionsDelegate.Value.SpineFhirApiKey);

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
