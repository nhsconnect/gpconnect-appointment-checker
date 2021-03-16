using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService : IGpConnectQueryExecutionService
    {
        private readonly ILogger<GpConnectQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IAuditService _auditService;
        private readonly IConfigurationService _configurationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private SpineMessage _spineMessage;

        public GpConnectQueryExecutionService(ILogger<GpConnectQueryExecutionService> logger, IConfigurationService configurationService, ILogService logService, IHttpClientFactory httpClientFactory, IAuditService auditService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
            _httpClientFactory = httpClientFactory;
            _auditService = auditService;
        }

        public async Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress)
        {
            _spineMessage = new SpineMessage();
            var capabilityStatement = await GetCapabilityStatement(requestParameters, baseAddress);
            return capabilityStatement;
        }

        public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, bool includePastSlots)
        {
            _spineMessage = new SpineMessage();
            var freeSlots = await GetFreeSlots(requestParameters, startDate, endDate, baseAddress, includePastSlots);
            return freeSlots;
        }

        private static void AddRequiredRequestHeaders(RequestParameters requestParameters, HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Ssp-From", requestParameters.SspFrom);
            client.DefaultRequestHeaders.Add("Ssp-To", requestParameters.SspTo);
            client.DefaultRequestHeaders.Add("Ssp-InteractionID", requestParameters.InteractionId);
            client.DefaultRequestHeaders.Add("Ssp-TraceID", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestParameters.BearerToken);
        }

        private string AddSecureSpineProxy(string baseAddress, RequestParameters requestParameters)
        {
            return requestParameters.UseSSP ? AddScheme(requestParameters.SspHostname) + "/" + baseAddress : baseAddress;
        }

        private string AddScheme(string sspHostname)
        {
            return !sspHostname.StartsWith("https://") ? "https://" + sspHostname : sspHostname;
        }
    }
}
