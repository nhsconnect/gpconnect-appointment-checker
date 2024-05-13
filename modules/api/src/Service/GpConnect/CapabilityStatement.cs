using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class CapabilityStatement : ICapabilityStatement
{
    private readonly ILogger<CapabilityStatement> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly ISlotSearchDependencies _slotSearchDependencies;
    private readonly ILogService _logService;
    private readonly IHttpClientFactory _httpClientFactory;
    private SpineMessage _spineMessage;

    public CapabilityStatement(ILogger<CapabilityStatement> logger, IConfigurationService configurationService, IHttpClientFactory httpClientFactory, ISlotSearchDependencies slotSearchDependencies, ILogService logService)
    {
        _logger = logger;
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _slotSearchDependencies = slotSearchDependencies ?? throw new ArgumentNullException(nameof(slotSearchDependencies));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _httpClientFactory = httpClientFactory;
        _spineMessage = new SpineMessage();
    }

    public async Task<DTO.Response.GpConnect.CapabilityStatement> GetCapabilityStatement(RequestParameters requestParameters, string baseAddress, string httpClient, string? interactionId = null, TimeSpan? timeoutOverride = null)
    {
        var getRequest = new HttpRequestMessage();
        var stopWatch = new Stopwatch();

        try
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, interactionId);

            requestParameters.SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId;
            requestParameters.InteractionId = spineMessageType?.InteractionId;

            _spineMessage.SpineMessageTypeId = (int)requestParameters.SpineMessageTypeId;

            var client = _httpClientFactory.CreateClient(httpClient);
            
            client.Timeout = timeoutOverride ?? client.Timeout;

            _slotSearchDependencies.AddRequiredRequestHeaders(requestParameters, client);
            _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();

            getRequest.Method = HttpMethod.Get;
            getRequest.RequestUri = new Uri($"{requestParameters.EndpointAddressWithSpineSecureProxy}/metadata");
            _spineMessage.RequestPayload = getRequest.ToString();

            stopWatch.Start();
            var response = await client.SendAsync(getRequest);
            var responseStream = await response.Content.ReadAsStringAsync();

            _spineMessage.ResponsePayload = responseStream;
            _spineMessage.ResponseStatus = response.StatusCode.ToString();
            _spineMessage.ResponseHeaders = response.Headers.ToString();

            var capabilityStatement = default(DTO.Response.GpConnect.CapabilityStatement);

            if (responseStream.IsJson())
            {
                capabilityStatement = JsonConvert.DeserializeObject<DTO.Response.GpConnect.CapabilityStatement>(responseStream);
            }
            else
            {
                capabilityStatement = new DTO.Response.GpConnect.CapabilityStatement();
                capabilityStatement.Issue.Add(new Issue() { Details = new Detail() { Coding = new List<Coding>() { new() { Code = response.StatusCode.ToString(), Display = "Response was not valid" } } } });
            }
            return capabilityStatement;

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An error occurred in trying to execute a GET request - {getRequest}");
            throw;
        }
        finally
        {
            stopWatch.Stop();
            _spineMessage.RoundTripTimeMs = stopWatch.Elapsed.TotalMilliseconds;
            await _logService.AddSpineMessageLog(_spineMessage);
        }
    }
}