//using gpconnect_appointment_checker.DTO;
//using gpconnect_appointment_checker.DTO.Request.Application;
//using gpconnect_appointment_checker.DTO.Request.GpConnect;
//using gpconnect_appointment_checker.DTO.Request.Logging;
//using gpconnect_appointment_checker.DTO.Response.GpConnect;
//using gpconnect_appointment_checker.GPConnect.Interfaces;
//using gpconnect_appointment_checker.Helpers;
//using gpconnect_appointment_checker.Helpers.Enumerations;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;

//namespace gpconnect_appointment_checker.GPConnect
//{
//    public partial class GpConnectQueryExecutionService : IGpConnectQueryExecutionService
//    {
//        private readonly ILogger<GpConnectQueryExecutionService> _logger;
//        private readonly ILogService _logService;        
//        private readonly IAuditService _auditService;
//        private readonly IConfigurationService _configurationService;
//        private readonly IHttpClientFactory _httpClientFactory;
//        private SpineMessage _spineMessage;
//        private readonly DateTime _currentDateTime = DateTime.Now.TimeZoneConverter();

//        public GpConnectQueryExecutionService(ILogger<GpConnectQueryExecutionService> logger, IConfigurationService configurationService, ILogService logService, IHttpClientFactory httpClientFactory, IAuditService auditService)
//        {
//            _logger = logger;
//            _configurationService = configurationService;
//            _logService = logService;
//            _httpClientFactory = httpClientFactory;
//            _auditService = auditService;
//        }

//        public Task<List<CapabilityStatementList>> ExecuteFhirCapabilityStatement(List<RequestParametersList> requestParameterList)
//        {
//            _logger.LogInformation("Executing ExecuteFhirCapabilityStatement");

//            var tokenSource = new CancellationTokenSource();
//            var token = tokenSource.Token;
//            _spineMessage = new SpineMessage();
//            var capabilityStatement = GetCapabilityStatement(requestParameterList, token);
//            return capabilityStatement;
//        }

//        public async Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress)
//        {
//            _spineMessage = new SpineMessage();
//            var capabilityStatement = await GetCapabilityStatement(requestParameters, baseAddress);
//            return capabilityStatement;
//        }

//        public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int userId)
//        {
//            _spineMessage = new SpineMessage();
//            var freeSlots = await GetFreeSlots(requestParameters, startDate, endDate, baseAddress);

//            var searchExport = new SearchExport
//            {
//                SearchExportData = freeSlots.ExportStreamData,
//                UserId = userId
//            };

//            var searchExportInstance = _applicationService.AddSearchExport(searchExport);
//            freeSlots.SearchExportId = searchExportInstance.SearchExportId;
//            return freeSlots;
//        }

//        public SlotSimple ExecuteFreeSlotSearchFromDatabase(string responseStream, int userId)
//        {
//            var freeSlots = GetFreeSlotsFromDatabase(responseStream);
//            var searchExport = new SearchExport
//            {
//                SearchExportData = freeSlots.ExportStreamData,
//                UserId = userId
//            };

//            var searchExportInstance = _applicationService.AddSearchExport(searchExport);
//            freeSlots.SearchExportId = searchExportInstance.SearchExportId;
//            return freeSlots;
//        }

//        public Task<List<SlotEntrySummaryCount>> ExecuteFreeSlotSearchSummary(List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetails, List<RequestParametersList> requestParameterList, DateTime startDate, DateTime endDate, SearchType searchType)
//        {
//            var tokenSource = new CancellationTokenSource();
//            var token = tokenSource.Token;
//            _spineMessage = new SpineMessage();
//            var freeSlotsSummary = GetFreeSlotsSummary(organisationErrorCodeOrDetails, requestParameterList, startDate, endDate, token, searchType);
//            return freeSlotsSummary;
//        }

//        private static void AddRequiredRequestHeaders(RequestParameters requestParameters, HttpClient client)
//        {
//            client.DefaultRequestHeaders.Remove("Ssp-From");
//            client.DefaultRequestHeaders.Remove("Ssp-To");
//            client.DefaultRequestHeaders.Remove("Ssp-InteractionID");
//            client.DefaultRequestHeaders.Remove("Ssp-TraceID");
//            client.DefaultRequestHeaders.Remove("Bearer");

//            client.DefaultRequestHeaders.Add("Ssp-From", requestParameters.SspFrom);
//            client.DefaultRequestHeaders.Add("Ssp-To", requestParameters.SspTo);
//            client.DefaultRequestHeaders.Add("Ssp-InteractionID", requestParameters.InteractionId);
//            client.DefaultRequestHeaders.Add("Ssp-TraceID", Guid.NewGuid().ToString());
//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestParameters.BearerToken);
//        }
//    }
//}
