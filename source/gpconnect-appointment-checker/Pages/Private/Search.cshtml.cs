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
        protected readonly ILoggerManager _loggerManager;
        protected Stopwatch _stopwatch = new Stopwatch();
        protected List<string> _auditSearchParameters = new List<string>(new[] { "", "", "" });
        protected List<string> _auditSearchIssues = new List<string>();
        protected bool _multiSearchEnabled;
        protected List<SlotEntrySummary> _searchResultsSummaryDataTable;

        public SearchModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ILdapService ldapService, ITokenService tokenService, IGpConnectQueryExecutionService queryExecutionService, IApplicationService applicationService, IAuditService auditService, IReportingService reportingService, ILoggerManager loggerManager = null)
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
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }

            if (_contextAccessor.HttpContext != null)
                _multiSearchEnabled = _contextAccessor.HttpContext.User.GetClaimValue("MultiSearchEnabled")
                    .StringToBoolean(false);
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
            return Page();
        }

        public FileStreamResult OnPostExportReport(int searchgroupid)
        {
            var memoryStream = _reportingService.ExportReport(searchgroupid, ReportConstants.SLOTSUMMARYREPORTHEADING);
            return new FileStreamResult(memoryStream,
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = $"{DateTime.UtcNow.ToFileTimeUtc()}.xlsx"
            };
        }

        private void PopulateSearchResultsForGroup(int searchGroupId, int userId)
        {
            var searchResultsForGroup = _applicationService.GetSearchResultByGroup(searchGroupId, userId);
            SearchResultsSummary = searchResultsForGroup;
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            if (ModelState.IsValid)
            {
                ProviderOdsCode = CleansedProviderOdsCodeInput;
                ConsumerOdsCode = CleansedConsumerOdsCodeInput;
                _stopwatch.Start();
                if (IsMultiSearch && ValidSearchCombination)
                {
                    GetSearchResultsMulti();
                }
                else if (!IsMultiSearch)
                {
                    await GetSearchResults();
                }
                _stopwatch.Stop();
                SearchDuration = _stopwatch.Elapsed.TotalSeconds;
                _queryExecutionService.SendToAudit(_auditSearchParameters, _auditSearchIssues, _stopwatch, SearchResultsCount);
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            ProviderOdsCode = null;
            ConsumerOdsCode = null;
            SelectedDateRange = DateRanges.First().Value;
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

                ProviderODSCodeFound = providerOrganisationDetails != null;
                ConsumerODSCodeFound = consumerOrganisationDetails != null;

                if (ProviderODSCodeFound && ConsumerODSCodeFound)
                {
                    //Step 2 - VALIDATE PROVIDER ODS CODE IN SPINE DIRECTORY
                    //Is ODS code configured in Spine Directory as an GP Connect Appointments provider system? / Retrieve provider endpoint and party key from Spine Directory
                    var providerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ProviderOdsCode);
                    var consumerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ConsumerOdsCode);
                    ProviderEnabledForGpConnectAppointmentManagement = providerGpConnectDetails != null;

                    if (ProviderEnabledForGpConnectAppointmentManagement && consumerOrganisationDetails != null)
                    {
                        var providerAsId = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(ProviderOdsCode, providerGpConnectDetails.party_key);
                        ProviderASIDPresent = providerAsId != null;

                        if (ProviderASIDPresent)
                        {
                            providerGpConnectDetails.asid = providerAsId.asid;
                            await PopulateSearchResults(providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                            SearchAtResultsText = $"{providerOrganisationDetails.OrganisationName} ({providerOrganisationDetails.ODSCode}) - {StringExtensions.AddressBuilder(providerOrganisationDetails.PostalAddressFields.ToList(), providerOrganisationDetails.PostalCode)}";
                            SearchOnBehalfOfResultsText = $"{consumerOrganisationDetails.OrganisationName} ({consumerOrganisationDetails.ODSCode}) - {StringExtensions.AddressBuilder(consumerOrganisationDetails.PostalAddressFields.ToList(), consumerOrganisationDetails.PostalCode)}";
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

        private void GetSearchResultsMulti()
        {
            try
            {
                var providerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ProviderOdsCodeAsList, ErrorCode.ProviderODSCodeNotFound);
                var consumerOrganisationDetails = _ldapService.GetOrganisationDetailsByOdsCode(ConsumerOdsCodeAsList, ErrorCode.ConsumerODSCodeNotFound);

                _auditSearchParameters[0] = ConsumerOdsCode;
                _auditSearchParameters[1] = ProviderOdsCode;
                _auditSearchParameters[2] = SelectedDateRange;

                var providerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ProviderOdsCodeAsList, ErrorCode.ProviderNotEnabledForGpConnectAppointmentManagement);
                var consumerGpConnectDetails = _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ConsumerOdsCodeAsList, ErrorCode.None);

                providerGpConnectDetails = _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(providerGpConnectDetails);

                var slotEntrySummary = PopulateSearchResultsMulti(providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                SearchResultsSummary = slotEntrySummary;
                _searchResultsSummaryDataTable = slotEntrySummary;
            }
            catch (LdapException)
            {
                LdapErrorRaised = true;
                _auditSearchIssues.Add(SearchConstants.ISSUEWITHLDAPTEXT);
            }
        }

        private async Task PopulateSearchResults(Spine providerGpConnectDetails, Organisation providerOrganisationDetails, Spine consumerGpConnectDetails, Organisation consumerOrganisationDetails)
        {
            var requestParameters = _tokenService.ConstructRequestParameters(
                _contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerGpConnectDetails, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots);
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

        private List<SlotEntrySummary> PopulateSearchResultsMulti(List<SpineList> providerGpConnectDetails, List<OrganisationList> providerOrganisationDetails,
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

            var requestParameters = _tokenService.ConstructRequestParameters(_contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerGpConnectDetails, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots);
            var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
            var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);

            var providerOdsCount = ProviderOdsCodeAsList.Count;
            var consumerOdsCount = ConsumerOdsCodeAsList.Count;
            List<SlotEntrySummaryCount> slotSearchSummaryList = null;

            if (providerOdsCount > consumerOdsCount)
            {
                for (var i = 0; i < providerOdsCount; i++)
                {
                    _stopwatch.Start();
                    var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[i], ConsumerOdsCodeAsList[0], providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                    var slotCount = 0;

                    if (errorCodeOrDetail.Item1 == ErrorCode.None)
                    {
                        if (requestParameters != null)
                        {
                            var capabilityStatementList = _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);
                            var capabilityStatementErrorCodeOrDetail = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[i], capabilityStatementList);

                            errorCodeOrDetail.Item1 = capabilityStatementErrorCodeOrDetail.Item1;
                            errorCodeOrDetail.Item2 = capabilityStatementErrorCodeOrDetail.Item2;

                            if (capabilityStatementErrorCodeOrDetail.Item1 == ErrorCode.None)
                            {                                
                                slotSearchSummaryList = _queryExecutionService.ExecuteFreeSlotSearchSummary(requestParameters, startDate, endDate);
                                var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ProviderOdsCodeAsList[i], slotSearchSummaryList);

                                errorCodeOrDetail.Item1 = slotSearchErrorCodeOrDetail.Item1;
                                errorCodeOrDetail.Item2 = slotSearchErrorCodeOrDetail.Item2;

                                slotCount = slotSearchErrorCodeOrDetail.Item3;
                            }
                        }
                    }
                    _stopwatch.Stop();

                    var searchResultToAdd = new SearchResult
                    {
                        SearchGroupId = createdSearchGroup.SearchGroupId,
                        ProviderCode = ProviderOdsCodeAsList[i],
                        ConsumerCode = ConsumerOdsCodeAsList[0],
                        ErrorCode = (int)errorCodeOrDetail.Item1,
                        Details = errorCodeOrDetail.Item2,
                        ProviderPublisher = errorCodeOrDetail.Item5?.product_name,
                        SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds
                    };

                    _stopwatch.Reset();

                    if (slotSearchSummaryList != null)
                    {
                        var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ProviderOdsCodeAsList[i]).SpineMessageId;
                        searchResultToAdd.SpineMessageId = spineMessageId;
                    }

                    var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

                    slotEntrySummary.Add(new SlotEntrySummary
                    {
                        ProviderLocationName = $"{errorCodeOrDetail.Item3?.OrganisationName}, {StringExtensions.AddressBuilder(errorCodeOrDetail.Item3?.PostalAddressFields.ToList(), errorCodeOrDetail.Item3?.PostalCode)}",
                        ProviderOdsCode = ProviderOdsCodeAsList[i],
                        ConsumerLocationName = $"{errorCodeOrDetail.Item4?.OrganisationName}, {StringExtensions.AddressBuilder(errorCodeOrDetail.Item4?.PostalAddressFields.ToList(), errorCodeOrDetail.Item4?.PostalCode)}",
                        ConsumerOdsCode = ConsumerOdsCodeAsList[0],
                        SearchSummaryDetail = errorCodeOrDetail.Item2,
                        ProviderPublisher = errorCodeOrDetail.Item5?.product_name,
                        SearchResultId = searchResult.SearchResultId,
                        DetailsEnabled = (errorCodeOrDetail.Item1 == ErrorCode.None && slotCount > 0),
                        DisplayProvider = errorCodeOrDetail.Item3 != null,
                        DisplayConsumer = errorCodeOrDetail.Item4 != null,
                        DisplayClass = (errorCodeOrDetail.Item1 != ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
                    });
                }
            }
            else if(consumerOdsCount > providerOdsCount)
            {
                for (var i = 0; i < consumerOdsCount; i++)
                {
                    _stopwatch.Start();
                    var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[0], ConsumerOdsCodeAsList[i], providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                    var slotCount = 0;

                    if (errorCodeOrDetail.Item1 == ErrorCode.None)
                    {
                        if (requestParameters != null)
                        {
                            var capabilityStatementList = _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);
                            var capabilityStatementErrorCodeOrDetail = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[0], capabilityStatementList);

                            errorCodeOrDetail.Item1 = capabilityStatementErrorCodeOrDetail.Item1;
                            errorCodeOrDetail.Item2 = capabilityStatementErrorCodeOrDetail.Item2;

                            if (capabilityStatementErrorCodeOrDetail.Item1 == ErrorCode.None)
                            {
                                slotSearchSummaryList = _queryExecutionService.ExecuteFreeSlotSearchSummary(requestParameters, startDate, endDate);
                                var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ProviderOdsCodeAsList[0], slotSearchSummaryList);

                                errorCodeOrDetail.Item1 = slotSearchErrorCodeOrDetail.Item1;
                                errorCodeOrDetail.Item2 = slotSearchErrorCodeOrDetail.Item2;

                                slotCount = slotSearchErrorCodeOrDetail.Item3;

                            }
                        }
                    }
                    _stopwatch.Stop();

                    var searchResultToAdd = new SearchResult
                    {
                        SearchGroupId = createdSearchGroup.SearchGroupId,
                        ProviderCode = ProviderOdsCodeAsList[0],
                        ConsumerCode = ConsumerOdsCodeAsList[i],
                        ErrorCode = (int)errorCodeOrDetail.Item1,
                        Details = errorCodeOrDetail.Item2,
                        ProviderPublisher = errorCodeOrDetail.Item5?.product_name,
                        SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds
                    };

                    _stopwatch.Reset();

                    if (slotSearchSummaryList != null)
                    {
                        var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ProviderOdsCodeAsList[0]).SpineMessageId;
                        searchResultToAdd.SpineMessageId = spineMessageId;
                    }

                    var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

                    slotEntrySummary.Add(new SlotEntrySummary
                    {
                        ProviderLocationName = $"{errorCodeOrDetail.Item3?.OrganisationName}, {StringExtensions.AddressBuilder(errorCodeOrDetail.Item3?.PostalAddressFields.ToList(), errorCodeOrDetail.Item3?.PostalCode)}",
                        ProviderOdsCode = ProviderOdsCodeAsList[0],
                        ConsumerLocationName = $"{errorCodeOrDetail.Item4?.OrganisationName}, {StringExtensions.AddressBuilder(errorCodeOrDetail.Item4?.PostalAddressFields.ToList(), errorCodeOrDetail.Item4?.PostalCode)}",
                        ConsumerOdsCode = ConsumerOdsCodeAsList[i],
                        SearchSummaryDetail = errorCodeOrDetail.Item2,
                        ProviderPublisher = errorCodeOrDetail.Item5?.product_name,
                        SearchResultId = searchResult.SearchResultId,
                        DetailsEnabled = (errorCodeOrDetail.Item1 == ErrorCode.None && slotCount > 0),
                        DisplayProvider = errorCodeOrDetail.Item3 != null,
                        DisplayConsumer = errorCodeOrDetail.Item4 != null
                    });
                }
            }
            return slotEntrySummary.OrderBy(x => x.SearchResultId).ToList();
        }

        private (ErrorCode, string, int) GetSlotSearchErrorCodeOrDetail(string providerOdsCode, List<SlotEntrySummaryCount> slotEntrySummaries)
        {
            var slotEntrySummary = slotEntrySummaries.FirstOrDefault(x => x.OdsCode == providerOdsCode);
            var errorSource = slotEntrySummary?.ErrorCode ?? ErrorCode.None;
            var detail = string.Empty;

            if (errorSource == ErrorCode.None)
            {
                if (slotEntrySummary != null && slotEntrySummary.FreeSlotCount.GetValueOrDefault() > 0)
                {
                    detail = string.Format(SearchConstants.FREESLOTSFOUNDTEXT, slotEntrySummary.FreeSlotCount.GetValueOrDefault());
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

        private (ErrorCode, string) GetCapabilityStatementErrorCodeOrDetail(string providerOdsCode, List<CapabilityStatementList> providerCapabilityStatements)
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
            return (errorSource, details);
        }

        private (ErrorCode, string, Organisation, Organisation, Spine) GetOrganisationErrorCodeOrDetail(string providerOdsCode, string consumerOdsCode, List<SpineList> providerGpConnectDetails, List<OrganisationList> providerOrganisationDetails,
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
            else
            {
                errorSource = consumerErrorCode;
                details = string.Format(errorSource.GetDescription(), consumerOdsCode);
            }

            return (errorSource, details, providerOrganisation, consumerOrganisation, providerSpine);
        }

        private int GetMaxNumberOfCodesForMultiSearch()
        {
            var maxNumberOfCodesForMultiSearch = _configuration["General:max_number_provider_codes_search"].StringToInteger(20);
            return maxNumberOfCodesForMultiSearch;
        }

        private List<SelectListItem> GetDateRanges()
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
    }
}
