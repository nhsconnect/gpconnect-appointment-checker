using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Enumerations;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Organisation = gpconnect_appointment_checker.DTO.Response.Application.Organisation;
using SearchResult = gpconnect_appointment_checker.DTO.Request.Application.SearchResult;
using gpconnect_appointment_checker.DTO;
using System.Text;
using System.IO;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel : PageModel
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchModel> _logger;
        protected ILdapService _ldapService;
        protected IApplicationService _applicationService;
        protected ITokenService _tokenService;
        protected IGpConnectQueryExecutionService _queryExecutionService;
        protected IAuditService _auditService;
        protected IReportingService _reportingService;
        protected IConfigurationService _configurationService;
        protected readonly ILoggerManager _loggerManager;
        protected Stopwatch _stopwatch = new Stopwatch();
        protected List<string> _auditSearchParameters = new List<string>(new[] { "", "", "", "" });
        protected List<string> _auditSearchIssues = new List<string>();
        protected bool _multiSearchEnabled;
        protected bool _orgTypeSearchEnabled;
        protected List<SlotEntrySummary> _searchResultsSummaryDataTable;

        public SearchModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ILdapService ldapService, ITokenService tokenService, IGpConnectQueryExecutionService queryExecutionService, IApplicationService applicationService, IAuditService auditService, IReportingService reportingService, IConfigurationService configurationService, ILoggerManager loggerManager = null)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _ldapService = ldapService;
            _tokenService = tokenService;
            _queryExecutionService = queryExecutionService;
            _applicationService = applicationService;
            _auditService = auditService;
            _reportingService = reportingService;
            _configurationService = configurationService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }

            if (_contextAccessor.HttpContext != null)
            {
                _multiSearchEnabled = _contextAccessor.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false);
                _orgTypeSearchEnabled = _contextAccessor.HttpContext.User.GetClaimValue("OrgTypeSearchEnabled").StringToBoolean(false);
            }
        }

        public IActionResult OnGet(string providerOdsCode, string consumerOdsCode)
        {
            if (!string.IsNullOrEmpty(providerOdsCode) && !string.IsNullOrEmpty(consumerOdsCode))
            {
                ProviderOdsCode = providerOdsCode;
                ConsumerOdsCode = consumerOdsCode;
            }
            else
            {
                var userCode = User.GetClaimValue("ODS");
                if (!string.IsNullOrEmpty(userCode)) ProviderOdsCode = userCode;
            }

            ModelState.ClearValidationState("ProviderOdsCode");
            ModelState.ClearValidationState("ConsumerOdsCode");
            return Page();
        }

        public IActionResult OnGetSearchByGroup(int searchGroupId)
        {
            var userId = User.GetClaimValue("UserId").StringToInteger();
            var searchGroup = _applicationService.GetSearchGroup(searchGroupId, userId);
            if (searchGroup != null)
            {
                ProviderOdsCode = searchGroup.ProviderOdsTextbox;
                ConsumerOdsCode = searchGroup.ConsumerOdsTextbox;
                SelectedDateRange = searchGroup.SelectedDateRange;
                PopulateSearchResultsForGroup(searchGroupId, userId);
            }
            ModelState.ClearValidationState("ProviderOdsCode");
            ModelState.ClearValidationState("ConsumerOdsCode");
            return Page();
        }

        public FileStreamResult OnPostExportSearchResults()
        {
            //var memoryStream = _reportingService.ExportReport(searchgroupid, ReportConstants.SLOTSUMMARYREPORTHEADING);
            MemoryStream memoryStream = null;
            return GetFileStream(memoryStream);
        }

        public FileStreamResult OnPostExportSearchGroupResults(int searchgroupid)
        {
            var memoryStream = _reportingService.ExportReport(searchgroupid, ReportConstants.SLOTSUMMARYREPORTHEADING);
            return GetFileStream(memoryStream);
        }

        private static FileStreamResult GetFileStream(MemoryStream memoryStream, string fileName = null)
        {
            return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = fileName ?? $"{DateTime.UtcNow.ToFileTimeUtc()}.xlsx"
            };
        }

        private void PopulateSearchResultsForGroup(int searchGroupId, int userId)
        {
            var searchResultsForGroup = _applicationService.GetSearchResultByGroup(searchGroupId, userId);
            SearchResultsSummary = searchResultsForGroup;
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            CheckConsumerInputs();
            if (ModelState.IsValid)
            {
                ProviderOdsCode = CleansedProviderOdsCodeInput;
                ConsumerOdsCode = CleansedConsumerOdsCodeInput;

                _stopwatch.Start();
                if (IsMultiSearch && ValidSearchCombination)
                {
                    SearchResultsSummary = await GetSearchResultsMulti();
                }
                else if (!IsMultiSearch)
                {
                    await GetSearchResults();
                }
                _stopwatch.Stop();
                SearchDuration = _stopwatch.Elapsed.TotalSeconds;
                _queryExecutionService.SendToAudit(_auditSearchParameters, _auditSearchIssues, _stopwatch, IsMultiSearch, SearchResultsCount);
            }

            return Page();
        }

        private void CheckConsumerInputs()
        {
            if (_orgTypeSearchEnabled && string.IsNullOrEmpty(ConsumerOdsCode) && string.IsNullOrEmpty(SelectedOrganisationType))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.CONSUMERODSCODENOTENTEREDERRORMESSAGE);
                ModelState.AddModelError("SelectedOrganisationType", SearchConstants.CONSUMERORGTYPENOTENTEREDERRORMESSAGE);
            }
            if (!_orgTypeSearchEnabled && string.IsNullOrEmpty(ConsumerOdsCode))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.CONSUMERODSCODEREQUIREDERRORMESSAGE);
            }
        }

        public IActionResult OnPostClear()
        {
            ProviderOdsCode = null;
            ConsumerOdsCode = null;
            SelectedDateRange = DateRanges.First().Value;
            SelectedOrganisationType = OrganisationTypes.First().Value;
            ModelState.Clear();
            return Page();
        }

        private async Task GetSearchResults()
        {
            try
            {
                var providerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ProviderOdsCode);
                var consumerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ConsumerOdsCode);

                _auditSearchParameters[0] = ConsumerOdsCode;
                _auditSearchParameters[1] = ProviderOdsCode;
                _auditSearchParameters[2] = SelectedDateRange;
                _auditSearchParameters[3] = SelectedOrganisationType;

                ProviderODSCodeFound = providerOrganisationDetails != null;
                ConsumerODSCodeFound = consumerOrganisationDetails != null;

                if (ProviderODSCodeFound && (ConsumerODSCodeFound || SelectedOrganisationType != null))
                {
                    //Step 2 - VALIDATE PROVIDER ODS CODE IN SPINE DIRECTORY
                    //Is ODS code configured in Spine Directory as an GP Connect Appointments provider system? / Retrieve provider endpoint and party key from Spine Directory
                    var providerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ProviderOdsCode);
                    var consumerEnablement = _ldapService.GetGpConsumerAsIdByOdsCode(ConsumerOdsCode);
                    ProviderEnabledForGpConnectAppointmentManagement = providerGpConnectDetails != null;
                    ConsumerEnabledForGpConnectAppointmentManagement = (consumerEnablement != null || SelectedOrganisationType != null);

                    if (ProviderEnabledForGpConnectAppointmentManagement/* && (consumerOrganisationDetails != null || SelectedOrganisationType != null)*/)
                    {
                        var providerAsId = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(ProviderOdsCode, providerGpConnectDetails.party_key);
                        ProviderASIDPresent = providerAsId != null;

                        if (ProviderASIDPresent)
                        {
                            providerGpConnectDetails.asid = providerAsId.asid;
                            await PopulateSearchResults(providerGpConnectDetails, providerOrganisationDetails, consumerEnablement, consumerOrganisationDetails, SelectedOrganisationType);
                            SearchAtResultsText = $"{providerOrganisationDetails.OrganisationName} ({providerOrganisationDetails.ODSCode}) - {StringExtensions.AddressBuilder(providerOrganisationDetails.PostalAddressFields.ToList(), providerOrganisationDetails.PostalCode)}";
                            SearchOnBehalfOfResultsText = GetSearchOnBehalfOfResultsText(consumerOrganisationDetails, SelectedOrganisationType);
                            ProviderPublisher = providerAsId.product_name;
                        }
                        else
                        {
                            _auditSearchIssues.Add(SearchConstants.ISSUEWITHGPCONNECTPROVIDERTEXT);
                        }
                    }
                    else
                    {
                        _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHGPCONNECTPROVIDERNOTENABLEDTEXT, ProviderOdsCode));
                    }
                }
                else
                {
                    if (!ProviderODSCodeFound) _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHPROVIDERODSCODETEXT, ProviderOdsCode));
                    if (!ConsumerODSCodeFound) _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHCONSUMERODSCODETEXT, ConsumerOdsCode));
                }
            }
            catch (LdapException)
            {
                LdapErrorRaised = true;
                _auditSearchIssues.Add(SearchConstants.ISSUEWITHLDAPTEXT);
            }
        }

        private string GetSearchOnBehalfOfResultsText(Organisation consumerOrganisationDetails, string selectedOrganisationType)
        {
            var searchOnBehalfOfResultsText = new StringBuilder();
            var selectedOrganisationTypeText = OrganisationTypes.FirstOrDefault(x => x.Value == selectedOrganisationType).Text;

            if (consumerOrganisationDetails != null)
            {
                searchOnBehalfOfResultsText.Append($"<p>{consumerOrganisationDetails.OrganisationName} ({consumerOrganisationDetails.ODSCode}) - {StringExtensions.AddressBuilder(consumerOrganisationDetails.PostalAddressFields.ToList(), consumerOrganisationDetails.PostalCode)}</p>");
            }
            if (!string.IsNullOrEmpty(selectedOrganisationType))
            {
                searchOnBehalfOfResultsText.Append($"<p>{string.Format(SearchConstants.SEARCHRESULTSSEARCHONBEHALFOFORGTYPETEXT, selectedOrganisationTypeText)}</p>");
            }
            return searchOnBehalfOfResultsText.ToString();
        }

        private async Task<List<SlotEntrySummary>> GetSearchResultsMulti()
        {
            List<SlotEntrySummary> slotEntrySummary = new List<SlotEntrySummary>();
            try
            {
                var providerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ProviderOdsCodeAsList, ErrorCode.ProviderODSCodeNotFound);
                var consumerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ConsumerOdsCodeAsList, ErrorCode.ConsumerODSCodeNotFound);

                _auditSearchParameters[0] = ConsumerOdsCode;
                _auditSearchParameters[1] = ProviderOdsCode;
                _auditSearchParameters[2] = SelectedDateRange;

                var providerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ProviderOdsCodeAsList, ErrorCode.ProviderNotEnabledForGpConnectAppointmentManagement);
                var consumerEnablement = _ldapService.GetGpConsumerAsIdByOdsCode(ConsumerOdsCodeAsList, ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement);

                providerGpConnectDetails = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(providerGpConnectDetails);

                slotEntrySummary = await PopulateSearchResultsMulti(providerGpConnectDetails, providerOrganisationDetails, consumerEnablement, consumerOrganisationDetails);
                _searchResultsSummaryDataTable = slotEntrySummary;
            }
            catch (LdapException)
            {
                LdapErrorRaised = true;
                _auditSearchIssues.Add(SearchConstants.ISSUEWITHLDAPTEXT);
            }
            return slotEntrySummary;
        }

        private async Task PopulateSearchResults(Spine providerGpConnectDetails, Organisation providerOrganisationDetails, Spine consumerEnablement, Organisation consumerOrganisationDetails, string consumerOrganisationType = "")
        {
            var requestParameters = _tokenService.ConstructRequestParameters(
                _contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerEnablement, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots, consumerOrganisationType);
            var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
            var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);

            if (requestParameters != null)
            {
                var capabilityStatement = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters, providerGpConnectDetails.ssp_hostname);
                CapabilityStatementOk = (capabilityStatement.Issue?.Count == 0 || capabilityStatement.Issue == null);

                if (CapabilityStatementOk)
                {
                    var searchResults = await _queryExecutionService.ExecuteFreeSlotSearch(requestParameters, startDate, endDate, providerGpConnectDetails.ssp_hostname);
                    SlotSearchOk = searchResults?.Issue == null;

                    if (SlotSearchOk)
                    {
                        SearchResults = new List<List<SlotEntrySimple>>();
                        var locationGrouping = searchResults?.SlotEntrySimple.GroupBy(l => l.LocationName)
                            .Select(grp => grp.ToList()).ToList();
                        SearchResultsCount = searchResults?.SlotEntrySimple.Count;

                        if (locationGrouping != null)
                        {
                            SearchResults.AddRange(locationGrouping);
                        }
                    }
                    else
                    {
                        ProviderErrorDisplay = searchResults.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
                        ProviderErrorCode = searchResults.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
                        ProviderErrorDiagnostics = StringExtensions.Coalesce(searchResults.Issue.FirstOrDefault()?.Diagnostics, searchResults.Issue.FirstOrDefault()?.Details.Text);
                        _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT, ProviderErrorDisplay, ProviderErrorCode));
                    }
                }
                else
                {
                    if (capabilityStatement?.Issue != null)
                    {
                        ProviderErrorDisplay = capabilityStatement?.Issue?.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
                        ProviderErrorCode = capabilityStatement?.Issue?.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
                        ProviderErrorDiagnostics = StringExtensions.Coalesce(capabilityStatement?.Issue?.FirstOrDefault()?.Diagnostics, capabilityStatement?.Issue.FirstOrDefault()?.Details.Text);
                        _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT, ProviderErrorDisplay, ProviderErrorCode));
                    }
                }
            }
        }

        private async Task<List<SlotEntrySummary>> PopulateSearchResultsMulti(List<SpineList> providerGpConnectDetails, List<OrganisationList> providerOrganisationDetails,
            List<SpineList> consumerGpConnectDetails, List<OrganisationList> consumerOrganisationDetails)
        {
            var searchGroup = new DTO.Request.Application.SearchGroup
            {
                UserSessionId = User.GetClaimValue("UserSessionId").StringToInteger(),
                ProviderOdsTextbox = ProviderOdsCode,
                ConsumerOdsTextbox = ConsumerOdsCode,
                SearchDateRange = SelectedDateRange,
                SearchStartAt = DateTime.UtcNow
            };
            var createdSearchGroup = _applicationService.AddSearchGroup(searchGroup);
            SearchGroupId = createdSearchGroup.SearchGroupId;

            var slotEntrySummary = new List<SlotEntrySummary>();

            var requestParameters = await _tokenService.ConstructRequestParameters(_contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerGpConnectDetails, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots);
            var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
            var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);

            var providerOdsCount = ProviderOdsCodeAsList.Count;
            var consumerOdsCount = ConsumerOdsCodeAsList.Count;
            List<SlotEntrySummaryCount> slotSearchSummaryList = new List<SlotEntrySummaryCount>();
            List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetail = new List<OrganisationErrorCodeOrDetail>();
            List<CapabilityStatementErrorCodeOrDetail> capabilityStatementErrorCodeOrDetail = new List<CapabilityStatementErrorCodeOrDetail>();

            if (providerOdsCount > consumerOdsCount)
            {
                for (var i = 0; i < providerOdsCount; i++)
                {
                    _stopwatch.Start();
                    var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[i], ConsumerOdsCodeAsList[0], providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                    organisationErrorCodeOrDetail.Add(errorCodeOrDetail);

                    var capabilityStatementList = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);
                    var capabilityStatementResult = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[i], capabilityStatementList);
                    capabilityStatementErrorCodeOrDetail.Add(capabilityStatementResult);
                }

                var slotCount = 0;

                if (requestParameters != null && organisationErrorCodeOrDetail != null && capabilityStatementErrorCodeOrDetail != null)
                {
                    slotSearchSummaryList = await _queryExecutionService.ExecuteFreeSlotSearchSummary(organisationErrorCodeOrDetail, requestParameters, startDate, endDate, SearchType.Provider);

                    for (var i = 0; i < providerOdsCount; i++)
                    {
                        var organisationErrorCodeOrDetailForCode = organisationErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedProviderOdsCode == ProviderOdsCodeAsList[i]);
                        var capabilityStatementErrorCodeOrDetailForCode = capabilityStatementErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedProviderOdsCode == ProviderOdsCodeAsList[i]);

                        if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None)
                        {
                            var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ProviderOdsCodeAsList[i], slotSearchSummaryList);
                            organisationErrorCodeOrDetailForCode.errorSource = slotSearchErrorCodeOrDetail.Item1;
                            organisationErrorCodeOrDetailForCode.details = AppendAdditionalDetails(slotSearchErrorCodeOrDetail.Item2, organisationErrorCodeOrDetailForCode.additionalDetails);
                            slotCount = slotSearchErrorCodeOrDetail.Item3;
                        }
                        else if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && capabilityStatementErrorCodeOrDetailForCode.errorSource != ErrorCode.None)
                        {
                            organisationErrorCodeOrDetailForCode.errorSource = capabilityStatementErrorCodeOrDetailForCode.errorSource;
                            organisationErrorCodeOrDetailForCode.details = capabilityStatementErrorCodeOrDetailForCode.details;
                            slotCount = 0;
                        }

                        _stopwatch.Stop();

                        var searchResultToAdd = new SearchResult
                        {
                            SearchGroupId = createdSearchGroup.SearchGroupId,
                            ProviderCode = ProviderOdsCodeAsList[i],
                            ConsumerCode = ConsumerOdsCodeAsList[0],
                            ErrorCode = (int)organisationErrorCodeOrDetailForCode.errorSource,
                            Details = organisationErrorCodeOrDetailForCode.details,
                            ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.product_name,
                            SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds
                        };

                        _stopwatch.Reset();

                        if (slotSearchSummaryList != null)
                        {
                            var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ProviderOdsCodeAsList[i])?.SpineMessageId;
                            searchResultToAdd.SpineMessageId = spineMessageId;
                        }

                        var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

                        slotEntrySummary.Add(new SlotEntrySummary
                        {
                            ProviderLocationName = $"{organisationErrorCodeOrDetailForCode.providerOrganisation?.OrganisationName}, {StringExtensions.AddressBuilder(organisationErrorCodeOrDetailForCode.providerOrganisation?.PostalAddressFields.ToList(), organisationErrorCodeOrDetailForCode.providerOrganisation?.PostalCode)}",
                            ProviderOdsCode = ProviderOdsCodeAsList[i],
                            ConsumerLocationName = $"{organisationErrorCodeOrDetailForCode.consumerOrganisation?.OrganisationName}, {StringExtensions.AddressBuilder(organisationErrorCodeOrDetailForCode.consumerOrganisation?.PostalAddressFields.ToList(), organisationErrorCodeOrDetailForCode.consumerOrganisation?.PostalCode)}",
                            ConsumerOdsCode = ConsumerOdsCodeAsList[0],
                            SearchSummaryDetail = organisationErrorCodeOrDetailForCode.details,
                            ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.product_name,
                            SearchResultId = searchResult.SearchResultId,
                            SearchGroupId = searchResultToAdd.SearchGroupId,
                            DetailsEnabled = (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && slotCount > 0),
                            DisplayProvider = organisationErrorCodeOrDetailForCode.providerOrganisation != null,
                            DisplayConsumer = organisationErrorCodeOrDetailForCode.consumerOrganisation != null,
                            DisplayClass = (organisationErrorCodeOrDetailForCode.errorSource != ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
                        });
                    }
                }
            }
            else if (consumerOdsCount > providerOdsCount)
            {
                for (var i = 0; i < consumerOdsCount; i++)
                {
                    _stopwatch.Start();
                    var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[0], ConsumerOdsCodeAsList[i], providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                    organisationErrorCodeOrDetail.Add(errorCodeOrDetail);

                    var capabilityStatementList = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);
                    var capabilityStatementResult = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[0], capabilityStatementList);
                    capabilityStatementErrorCodeOrDetail.Add(capabilityStatementResult);
                }

                var slotCount = 0;

                if (requestParameters != null && organisationErrorCodeOrDetail != null && capabilityStatementErrorCodeOrDetail != null)
                {
                    slotSearchSummaryList = await _queryExecutionService.ExecuteFreeSlotSearchSummary(organisationErrorCodeOrDetail, requestParameters, startDate, endDate, SearchType.Consumer);

                    for (var i = 0; i < consumerOdsCount; i++)
                    {
                        var organisationErrorCodeOrDetailForCode = organisationErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedConsumerOdsCode == ConsumerOdsCodeAsList[i]);
                        var capabilityStatementErrorCodeOrDetailForCode = capabilityStatementErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedConsumerOdsCode == ConsumerOdsCodeAsList[i]);

                        if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None)
                        {
                            var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ConsumerOdsCodeAsList[i], slotSearchSummaryList);
                            organisationErrorCodeOrDetailForCode.errorSource = slotSearchErrorCodeOrDetail.Item1;
                            organisationErrorCodeOrDetailForCode.details = AppendAdditionalDetails(slotSearchErrorCodeOrDetail.Item2, organisationErrorCodeOrDetailForCode.additionalDetails);
                            slotCount = slotSearchErrorCodeOrDetail.Item3;
                        }
                        else if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && capabilityStatementErrorCodeOrDetailForCode.errorSource != ErrorCode.None)
                        {
                            organisationErrorCodeOrDetailForCode.errorSource = capabilityStatementErrorCodeOrDetailForCode.errorSource;
                            organisationErrorCodeOrDetailForCode.details = capabilityStatementErrorCodeOrDetailForCode.details;
                            slotCount = 0;
                        }

                        _stopwatch.Stop();

                        var searchResultToAdd = new SearchResult
                        {
                            SearchGroupId = createdSearchGroup.SearchGroupId,
                            ProviderCode = ProviderOdsCodeAsList[0],
                            ConsumerCode = ConsumerOdsCodeAsList[i],
                            ErrorCode = (int)organisationErrorCodeOrDetailForCode.errorSource,
                            Details = organisationErrorCodeOrDetailForCode.details,
                            ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.product_name,
                            SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds
                        };

                        _stopwatch.Reset();

                        if (slotSearchSummaryList != null)
                        {
                            var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ConsumerOdsCodeAsList[i])?.SpineMessageId;
                            searchResultToAdd.SpineMessageId = spineMessageId;
                        }

                        var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

                        slotEntrySummary.Add(new SlotEntrySummary
                        {
                            ProviderLocationName = $"{organisationErrorCodeOrDetailForCode.providerOrganisation?.OrganisationName}, {StringExtensions.AddressBuilder(organisationErrorCodeOrDetailForCode.providerOrganisation?.PostalAddressFields.ToList(), organisationErrorCodeOrDetailForCode.providerOrganisation?.PostalCode)}",
                            ProviderOdsCode = ProviderOdsCodeAsList[0],
                            ConsumerLocationName = $"{organisationErrorCodeOrDetailForCode.consumerOrganisation?.OrganisationName}, {StringExtensions.AddressBuilder(organisationErrorCodeOrDetailForCode.consumerOrganisation?.PostalAddressFields.ToList(), organisationErrorCodeOrDetailForCode.consumerOrganisation?.PostalCode)}",
                            ConsumerOdsCode = ConsumerOdsCodeAsList[i],
                            SearchSummaryDetail = organisationErrorCodeOrDetailForCode.details,
                            ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.product_name,
                            SearchResultId = searchResult.SearchResultId,
                            DetailsEnabled = (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && slotCount > 0),
                            DisplayProvider = organisationErrorCodeOrDetailForCode.providerOrganisation != null,
                            DisplayConsumer = organisationErrorCodeOrDetailForCode.consumerOrganisation != null,
                            DisplayClass = (organisationErrorCodeOrDetailForCode.errorSource != ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
                        });
                    }
                }
            }
            return slotEntrySummary.OrderBy(x => x.SearchResultId).ToList();
        }

        private string AppendAdditionalDetails(string item2, string additionalDetails)
        {
            return !string.IsNullOrEmpty(additionalDetails) ? $"{item2}<div class=\"nhsuk-warning-message\"><p>{additionalDetails}</p></div>" : item2;
        }

        private (ErrorCode, string, int) GetSlotSearchErrorCodeOrDetail(string providerOdsCode, List<SlotEntrySummaryCount> slotEntrySummaries)
        {
            var slotEntrySummary = slotEntrySummaries.FirstOrDefault(x => x.OdsCode == providerOdsCode);
            var errorSource = slotEntrySummary?.ErrorCode ?? ErrorCode.None;
            var detail = string.Empty;

            if (errorSource == ErrorCode.None || errorSource == ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement)
            {
                if (slotEntrySummary != null && slotEntrySummary.FreeSlotCount.GetValueOrDefault() > 0)
                {
                    detail = string.Format(SearchConstants.SEARCHSTATSCOUNTTEXT, slotEntrySummary.FreeSlotCount.GetValueOrDefault());
                }
                else
                {
                    detail = SearchConstants.SEARCHRESULTSNOAVAILABLEAPPOINTMENTSLOTSTEXT;
                }
            }
            else
            {
                var slotEntrySummaryIssueDetail = slotEntrySummary?.ErrorDetail.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
                var slotEntrySummaryIssueCode = slotEntrySummary?.ErrorDetail.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
                var slotEntrySummaryIssueDiagnostics = StringExtensions.Coalesce(slotEntrySummary?.ErrorDetail.FirstOrDefault()?.Diagnostics, slotEntrySummary?.ErrorDetail.FirstOrDefault()?.Details.Text);
                detail = StringExtensions.FlattenStrings(slotEntrySummaryIssueDetail, slotEntrySummaryIssueCode, slotEntrySummaryIssueDiagnostics);
            }
            return (errorSource, detail, slotEntrySummary.FreeSlotCount.GetValueOrDefault());
        }

        private CapabilityStatementErrorCodeOrDetail GetCapabilityStatementErrorCodeOrDetail(string providerOdsCode, List<CapabilityStatementList> providerCapabilityStatements)
        {
            var providerCapabilityStatement = providerCapabilityStatements.FirstOrDefault(x => x.OdsCode == providerOdsCode);
            var errorSource = providerCapabilityStatement?.ErrorCode ?? ErrorCode.None;
            var details = string.Empty;

            if (errorSource == ErrorCode.CapabilityStatementHasErrors)
            {
                details = StringExtensions.Coalesce(
                    providerCapabilityStatement?.CapabilityStatement.Issue?.FirstOrDefault()?.Diagnostics,
                    providerCapabilityStatement?.CapabilityStatement?.Issue.FirstOrDefault()?.Details.Text);
            }

            return new CapabilityStatementErrorCodeOrDetail
            {
                SuppliedProviderOdsCode = providerOdsCode,
                details = details,
                errorSource = errorSource
            };
        }

        private OrganisationErrorCodeOrDetail GetOrganisationErrorCodeOrDetail(string providerOdsCode, string consumerOdsCode, List<SpineList> providerGpConnectDetails, List<OrganisationList> providerOrganisationDetails,
            List<SpineList> consumerGpConnectDetails, List<OrganisationList> consumerOrganisationDetails)
        {
            var providerOrganisationLookupErrors = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            var consumerOrganisationLookupErrors = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;

            var providerGpConnectDetailsErrors = providerGpConnectDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            var consumerGpConnectDetailsErrors = consumerGpConnectDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;

            var providerErrorCode = providerOrganisationLookupErrors ?? providerGpConnectDetailsErrors ?? ErrorCode.None;
            var consumerErrorCode = consumerOrganisationLookupErrors ?? consumerGpConnectDetailsErrors ?? ErrorCode.None;

            var errorSource = ErrorCode.None;
            var details = string.Empty;
            var additionalDetails = string.Empty;
            Organisation providerOrganisation = null;
            Organisation consumerOrganisation = null;
            Spine providerSpine = null;

            if (providerErrorCode == ErrorCode.None)
            {
                providerOrganisation = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode)?.Organisation;
                providerSpine = providerGpConnectDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode)?.Spine;
            }
            else
            {
                errorSource = providerErrorCode;
                details = string.Format(errorSource.GetDescription(), providerOdsCode);
            }

            if (consumerErrorCode == ErrorCode.None)
            {
                consumerOrganisation = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode)?.Organisation;
            }
            else if (consumerErrorCode == ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement)
            {
                consumerOrganisation = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode)?.Organisation;
                additionalDetails = string.Format(consumerErrorCode.GetDescription(), consumerOdsCode);
            }
            else
            {
                errorSource = consumerErrorCode;
                details = string.Format(errorSource.GetDescription(), consumerOdsCode);
            }

            return new OrganisationErrorCodeOrDetail
            {
                SuppliedProviderOdsCode = providerOdsCode,
                SuppliedConsumerOdsCode = consumerOdsCode,
                errorSource = errorSource,
                details = details,
                additionalDetails = additionalDetails,
                providerOrganisation = providerOrganisation,
                consumerOrganisation = consumerOrganisation,
                providerSpine = providerSpine
            };
        }

        private int GetMaxNumberOfCodesForMultiSearch()
        {
            var maxNumberOfCodesForMultiSearch = _configuration["General:max_number_provider_codes_search"].StringToInteger(20);
            return maxNumberOfCodesForMultiSearch;
        }

        private IEnumerable<SelectListItem> GetDateRanges()
        {
            var weeksToGet = _configuration["General:max_num_weeks_search"].StringToInteger(12);
            var dateRange = new List<SelectListItem>();
            var firstDayOfCurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            for (var i = 0; i < weeksToGet; i++)
            {
                var week = new SelectListItem
                {
                    Text = $"{firstDayOfCurrentWeek:ddd d MMM} - {firstDayOfCurrentWeek.AddDays(6):ddd d MMM}",
                    Value = $"{firstDayOfCurrentWeek:d-MMM-yyyy}:{firstDayOfCurrentWeek.AddDays(6):d-MMM-yyyy}"
                };
                dateRange.Add(week);
                firstDayOfCurrentWeek = firstDayOfCurrentWeek.AddDays(7);
            }
            return dateRange;
        }

        private IEnumerable<SelectListItem> GetOrganisationTypes()
        {
            var organisationTypes = _configurationService.GetOrganisationTypes();
            var options = organisationTypes.Select(ot => new SelectListItem()
            {
                Text = ot.OrganisationTypeDescription,
                Value = ot.OrganisationTypeCode
            }).ToList();
            options.Insert(0, new SelectListItem());
            return options;
        }
    }
}
