using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DTO;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IApplicationService = GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces.IApplicationService;
using ITokenService = GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces.ITokenService;
using Organisation = gpconnect_appointment_checker.DTO.Response.Application.Organisation;
using SearchResult = gpconnect_appointment_checker.DTO.Request.Application.SearchResult;
using SlotEntrySummary = GpConnect.AppointmentChecker.Models.SlotEntrySummary;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel : SearchBaseModel
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<SearchModel> _logger;
        private readonly IApplicationService _applicationService;
        private readonly ITokenService _tokenService;
        private readonly ISpineService _spineService;
        private readonly IGpConnectQueryExecutionService _queryExecutionService;
        private readonly IReportingService _reportingService;
        private readonly IConfigurationService _configurationService;
        private readonly ILoggerManager _loggerManager;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly List<string> _auditSearchParameters = new List<string>(new[] { "", "", "", "" });
        private readonly List<string> _auditSearchIssues = new List<string>();
        private readonly List<SlotEntrySummary> _searchResultsSummaryDataTable;

        public SearchModel(IOptions<General> configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ITokenService tokenService, IGpConnectQueryExecutionService queryExecutionService, IApplicationService applicationService, IReportingService reportingService, IConfigurationService configurationService, ILoggerManager loggerManager = null) : base(configuration, contextAccessor, reportingService)
        {
            _contextAccessor = contextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _queryExecutionService = queryExecutionService;
            _applicationService = applicationService;
            _reportingService = reportingService;
            _configurationService = configurationService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
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

        public async Task<IActionResult> OnGetSearchByGroup(int searchGroupId)
        {
            var searchGroup = await _applicationService.GetSearchGroup(searchGroupId, UserId);
            if (searchGroup != null)
            {
                ProviderOdsCode = searchGroup.ProviderOdsTextbox;
                ConsumerOdsCode = searchGroup.ConsumerOdsTextbox;
                SelectedDateRange = searchGroup.SelectedDateRange;
                SelectedOrganisationType = searchGroup.ConsumerOrganisationTypeDropdown;
                PopulateSearchResultsForGroup(searchGroupId);
            }
            ModelState.ClearValidationState("ProviderOdsCode");
            ModelState.ClearValidationState("ConsumerOdsCode");
            ModelState.ClearValidationState("SelectedOrganisationType");
            return Page();
        }

        public async Task<FileStreamResult> OnPostExportSearchResults(int searchexportid)
        {
            var exportTable = await _applicationService.GetSearchExport(searchexportid, UserId);
            //return ExportResult(exportTable);
            return null;
        }

        public async Task<FileStreamResult> OnPostExportSearchGroupResults(int searchgroupid)
        {
            var exportTable = await _applicationService.GetSearchGroupExport(searchgroupid, UserId);
            //return ExportResult(exportTable);
            return null;
        }

        private async Task PopulateSearchResultsForGroup(int searchGroupId)
        {
            var searchResultsForGroup = await _applicationService.GetSearchResultByGroup(searchGroupId, UserId);
            SearchResultsSummary = searchResultsForGroup;
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            CheckInputs();
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
                _queryExecutionService.SendToAudit(_auditSearchParameters, _auditSearchIssues, _stopwatch, IsMultiSearch, SearchResultsTotalCount);
            }

            return Page();
        }

        private void CheckInputs()
        {
            if (OrgTypeSearchEnabled && (string.IsNullOrEmpty(ConsumerOdsCode) || ConsumerOdsCodeAsList?.Count == 0) && string.IsNullOrEmpty(SelectedOrganisationType))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.CONSUMERODSCODENOTENTEREDERRORMESSAGE);
                ModelState.AddModelError("SelectedOrganisationType", SearchConstants.CONSUMERORGTYPENOTENTEREDERRORMESSAGE);
            }
            if (!OrgTypeSearchEnabled && (string.IsNullOrEmpty(ConsumerOdsCode) || ConsumerOdsCodeAsList?.Count == 0))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.CONSUMERODSCODEREQUIREDERRORMESSAGE);
            }
            if ((string.IsNullOrEmpty(ProviderOdsCode) || ProviderOdsCodeAsList?.Count == 0))
            {
                ModelState.AddModelError("ProviderOdsCode", SearchConstants.PROVIDERODSCODEREQUIREDERRORMESSAGE);
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
                var providerOrganisationDetails = await _spineService.GetOrganisation(ProviderOdsCode);
                var consumerOrganisationDetails = await _spineService.GetOrganisation(ConsumerOdsCode);

                _auditSearchParameters[0] = ConsumerOdsCode;
                _auditSearchParameters[1] = ProviderOdsCode;
                _auditSearchParameters[2] = SelectedDateRange;
                _auditSearchParameters[3] = SelectedOrganisationType;

                ProviderODSCodeFound = providerOrganisationDetails != null;
                ConsumerODSCodeFound = consumerOrganisationDetails != null;

                if (ProviderODSCodeFound && (ConsumerODSCodeFound || SelectedOrganisationType != null))
                {
                    var providerSpineDetails = await _spineService.GetProviderDetails(ProviderOdsCode);
                    var consumerSpineDetails = await _spineService.GetConsumerDetails(ConsumerOdsCode);

                    ProviderEnabledForGpConnectAppointmentManagement = providerSpineDetails != null;
                    ConsumerEnabledForGpConnectAppointmentManagement = (consumerSpineDetails != null && consumerSpineDetails.HasAsId) || SelectedOrganisationType != null;

                    if (ProviderEnabledForGpConnectAppointmentManagement)
                    {
                        ProviderASIDPresent = providerSpineDetails.HasAsId;

                        if (ProviderASIDPresent)
                        {
                            await PopulateSearchResults(providerSpineDetails, providerOrganisationDetails, consumerSpineDetails, consumerOrganisationDetails, SelectedOrganisationType);
                            SearchAtResultsText = providerOrganisationDetails.FormattedOrganisationDetails;
                            SearchOnBehalfOfResultsText = GetSearchOnBehalfOfResultsText(consumerOrganisationDetails?.FormattedOrganisationDetails, SelectedOrganisationType);
                            ProviderPublisher = providerSpineDetails.ProductName;
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

        private string GetSearchOnBehalfOfResultsText(string consumerFormattedOrganisationDetails, string selectedOrganisationType)
        {
            var searchOnBehalfOfResultsText = new StringBuilder();
            var selectedOrganisationTypeText = OrganisationTypes.FirstOrDefault(x => x.Value == selectedOrganisationType).Text;

            if (!string.IsNullOrEmpty(consumerFormattedOrganisationDetails))
            {
                searchOnBehalfOfResultsText.Append($"<p>{consumerFormattedOrganisationDetails}</p>");
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
            //try
            //{
            //    var providerOrganisationDetails = await _sdsQueryExecutionBase.GetOrganisationDetailsByOdsCode(ProviderOdsCodeAsList, ErrorCode.ProviderODSCodeNotFound);
            //    var consumerOrganisationDetails = await _sdsQueryExecutionBase.GetOrganisationDetailsByOdsCode(ConsumerOdsCodeAsList, ErrorCode.ConsumerODSCodeNotFound);

            //    _auditSearchParameters[0] = ConsumerOdsCode;
            //    _auditSearchParameters[1] = ProviderOdsCode;
            //    _auditSearchParameters[2] = SelectedDateRange;
            //    _auditSearchParameters[3] = SelectedOrganisationType;

            //    var providerSpineDetails = await _sdsQueryExecutionBase.GetProviderDetails(ProviderOdsCodeAsList, ErrorCode.ProviderNotEnabledForGpConnectAppointmentManagement);
            //    var consumerSpineDetails = await _sdsQueryExecutionBase.GetConsumerDetails(ConsumerOdsCodeAsList, ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement);

            //    _logger.LogInformation("About to execute PopulateSearchResultsMulti");

            //    slotEntrySummary = await PopulateSearchResultsMulti(providerSpineDetails, providerOrganisationDetails, consumerSpineDetails, consumerOrganisationDetails, SelectedOrganisationType);
            //    _searchResultsSummaryDataTable = slotEntrySummary;
            //}
            //catch (LdapException)
            //{
            //    LdapErrorRaised = true;
            //    _auditSearchIssues.Add(SearchConstants.ISSUEWITHLDAPTEXT);
            //}
            return slotEntrySummary;
        }

        private async Task PopulateSearchResults(GpConnect.AppointmentChecker.Models.Spine providerSpineDetails, GpConnect.AppointmentChecker.Models.OrganisationSpine providerOrganisationDetails, GpConnect.AppointmentChecker.Models.Spine consumerEnablement, GpConnect.AppointmentChecker.Models.OrganisationSpine consumerOrganisationDetails, string consumerOrganisationType = "")
        {


            //var requestParameters = _tokenService.ConstructRequestParameters(
            //    _contextAccessor.HttpContext.GetAbsoluteUri(), providerSpineDetails, providerOrganisationDetails,
            //    consumerEnablement, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots, consumerOrganisationType);
            //var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
            //var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);

            //if (requestParameters != null)
            //{
            //    var capabilityStatement = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters, providerSpineDetails.SspHostname);
            //    CapabilityStatementOk = capabilityStatement.CapabilityStatementNoIssues;

            //    if (CapabilityStatementOk)
            //    {
            //        var searchResults = await _queryExecutionService.ExecuteFreeSlotSearch(requestParameters, startDate, endDate, providerSpineDetails.SspHostname, User.GetClaimValue("UserId").StringToInteger());

            //        SlotSearchOk = searchResults.SlotSearchNoIssues;

            //        if (SlotSearchOk)
            //        {
            //            SearchExportId = searchResults.SearchExportId;
            //            SearchResults = new List<List<SlotEntrySimple>>();
            //            SearchResultsPast = new List<List<SlotEntrySimple>>();

            //            SearchResultsTotalCount = searchResults.SlotCount;
            //            SearchResultsCurrentCount = searchResults.CurrentSlotCount;
            //            SearchResultsPastCount = searchResults.PastSlotCount;

            //            SearchResults.AddRange(searchResults.CurrentSlotEntriesByLocationGrouping);
            //            SearchResultsPast.AddRange(searchResults.PastSlotEntriesByLocationGrouping);
            //        }
            //        else
            //        {
            //            ProviderErrorDisplay = searchResults.ProviderError;
            //            ProviderErrorCode = searchResults.ProviderErrorCode;
            //            ProviderErrorDiagnostics = searchResults.ProviderErrorDiagnostics;
            //            _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT, ProviderErrorDisplay, ProviderErrorCode));
            //        }
            //    }
            //    else
            //    {
            //        ProviderErrorDisplay = capabilityStatement.ProviderError;
            //        ProviderErrorCode = capabilityStatement.ProviderErrorCode;
            //        ProviderErrorDiagnostics = capabilityStatement.ProviderErrorDiagnostics;
            //        _auditSearchIssues.Add(string.Format(SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT, ProviderErrorDisplay, ProviderErrorCode));

            //    }
            //}
        }

        private async Task<List<SlotEntrySummary>> PopulateSearchResultsMulti(List<SpineList> providerGpConnectDetails, List<OrganisationList> providerOrganisationDetails,
            List<SpineList> consumerGpConnectDetails, List<OrganisationList> consumerOrganisationDetails, string consumerOrganisationType = "")
        {
            //var createdSearchGroup = _applicationService.AddSearchGroup(new DTO.Request.Application.SearchGroup
            //{
            //    UserSessionId = User.GetClaimValue("UserSessionId").StringToInteger(),
            //    ProviderOdsTextbox = ProviderOdsCode,
            //    ConsumerOdsTextbox = ConsumerOdsCode,
            //    SearchDateRange = SelectedDateRange,
            //    ConsumerOrganisationTypeDropdown = SelectedOrganisationType
            //});

            //_logger.LogInformation("Executing PopulateSearchResultsMulti - generating Request Parameters");

            //SearchGroupId = createdSearchGroup.SearchGroupId;

            var slotEntrySummary = new List<SlotEntrySummary>();

            //var requestParameters = await _tokenService.ConstructRequestParameters(_contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
            //    consumerGpConnectDetails, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots, consumerOrganisationType);
            //var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
            //var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);

            //var providerOdsCount = ProviderOdsCodeAsList.Count;
            //var consumerOdsCount = ConsumerOdsCodeAsList.Count;
            //List<SlotEntrySummaryCount> slotSearchSummaryList = new List<SlotEntrySummaryCount>();
            //List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetail = new List<OrganisationErrorCodeOrDetail>();
            //List<CapabilityStatementErrorCodeOrDetail> capabilityStatementErrorCodeOrDetail = new List<CapabilityStatementErrorCodeOrDetail>();

            //if (providerOdsCount > consumerOdsCount)
            //{
            //    _logger.LogInformation("Executing PopulateSearchResultsMulti - Getting capability statement list");

            //    var consumerCode = ConsumerOdsCodeAsList.Count == 0 ? null : ConsumerOdsCodeAsList[0];
            //    var capabilityStatementList = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);

            //    for (var i = 0; i < providerOdsCount; i++)
            //    {
            //        _stopwatch.Start();
            //        var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[i], consumerCode, providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails, consumerOrganisationType);
            //        organisationErrorCodeOrDetail.Add(errorCodeOrDetail);

            //        var capabilityStatementResult = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[i], capabilityStatementList);
            //        capabilityStatementErrorCodeOrDetail.Add(capabilityStatementResult);
            //    }

            //    var slotCount = 0;

            //    if (requestParameters != null && organisationErrorCodeOrDetail != null && capabilityStatementErrorCodeOrDetail != null)
            //    {
            //        slotSearchSummaryList = await _queryExecutionService.ExecuteFreeSlotSearchSummary(organisationErrorCodeOrDetail, requestParameters, startDate, endDate, SearchType.Provider);

            //        for (var i = 0; i < providerOdsCount; i++)
            //        {
            //            var organisationErrorCodeOrDetailForCode = organisationErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedProviderOdsCode == ProviderOdsCodeAsList[i]);
            //            var capabilityStatementErrorCodeOrDetailForCode = capabilityStatementErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedProviderOdsCode == ProviderOdsCodeAsList[i]);

            //            if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None)
            //            {
            //                var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ProviderOdsCodeAsList[i], slotSearchSummaryList);
            //                organisationErrorCodeOrDetailForCode.errorSource = slotSearchErrorCodeOrDetail.Item1;
            //                organisationErrorCodeOrDetailForCode.details = AppendAdditionalDetails(slotSearchErrorCodeOrDetail.Item2, organisationErrorCodeOrDetailForCode.additionalDetails);
            //                slotCount = slotSearchErrorCodeOrDetail.Item3.GetValueOrDefault();
            //            }
            //            else if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && capabilityStatementErrorCodeOrDetailForCode.errorSource != ErrorCode.None)
            //            {
            //                organisationErrorCodeOrDetailForCode.errorSource = capabilityStatementErrorCodeOrDetailForCode.errorSource;
            //                organisationErrorCodeOrDetailForCode.details = capabilityStatementErrorCodeOrDetailForCode.details;
            //                slotCount = 0;
            //            }

            //            _stopwatch.Stop();

            //            var searchResultToAdd = new SearchResult
            //            {
            //                SearchGroupId = createdSearchGroup.SearchGroupId,
            //                ProviderCode = ProviderOdsCodeAsList[i],
            //                ConsumerCode = consumerCode,
            //                ConsumerOrganisationType = OrganisationTypes.FirstOrDefault(x => x.Value == consumerOrganisationType).Text,
            //                ErrorCode = (int)organisationErrorCodeOrDetailForCode.errorSource,
            //                Details = organisationErrorCodeOrDetailForCode.details,
            //                ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.ProductName,
            //                SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds
            //            };

            //            _stopwatch.Reset();

            //            if (slotSearchSummaryList != null)
            //            {
            //                var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ProviderOdsCodeAsList[i])?.SpineMessageId;
            //                searchResultToAdd.SpineMessageId = spineMessageId;
            //            }

            //            var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

            //            slotEntrySummary.Add(new SlotEntrySummary
            //            {
            //                ProviderLocationName = organisationErrorCodeOrDetailForCode.providerOrganisation?.OrganisationLocation,
            //                ProviderOdsCode = ProviderOdsCodeAsList[i],
            //                ConsumerLocationName = organisationErrorCodeOrDetailForCode.consumerOrganisation?.OrganisationLocation,
            //                ConsumerOdsCode = consumerCode,
            //                ConsumerOrganisationType = OrganisationTypes.FirstOrDefault(x => x.Value == consumerOrganisationType).Text,
            //                SearchSummaryDetail = organisationErrorCodeOrDetailForCode.details,
            //                ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.ProductName,
            //                SearchResultId = searchResult.SearchResultId,
            //                SearchGroupId = searchResultToAdd.SearchGroupId,
            //                DetailsEnabled = (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && slotCount > 0),
            //                DisplayProvider = organisationErrorCodeOrDetailForCode.providerOrganisation != null,
            //                DisplayConsumer = organisationErrorCodeOrDetailForCode.consumerOrganisation != null,
            //                DisplayClass = (organisationErrorCodeOrDetailForCode.errorSource != ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
            //            });
            //        }
            //    }
            //}
            //else if (consumerOdsCount > providerOdsCount)
            //{
            //    for (var i = 0; i < consumerOdsCount; i++)
            //    {
            //        _stopwatch.Start();
            //        var errorCodeOrDetail = GetOrganisationErrorCodeOrDetail(ProviderOdsCodeAsList[0], ConsumerOdsCodeAsList[i], providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails, consumerOrganisationType);
            //        organisationErrorCodeOrDetail.Add(errorCodeOrDetail);                    
            //    }

            //    var capabilityStatementList = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters);
            //    var capabilityStatementResult = GetCapabilityStatementErrorCodeOrDetail(ProviderOdsCodeAsList[0], capabilityStatementList);
            //    capabilityStatementErrorCodeOrDetail.Add(capabilityStatementResult);

            //    var slotCount = 0;

            //    if (requestParameters != null && organisationErrorCodeOrDetail != null && capabilityStatementErrorCodeOrDetail != null)
            //    {
            //        slotSearchSummaryList = await _queryExecutionService.ExecuteFreeSlotSearchSummary(organisationErrorCodeOrDetail, requestParameters, startDate, endDate, SearchType.Consumer);

            //        for (var i = 0; i < consumerOdsCount; i++)
            //        {
            //            var organisationErrorCodeOrDetailForCode = organisationErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedConsumerOdsCode == ConsumerOdsCodeAsList[i]);
            //            var capabilityStatementErrorCodeOrDetailForCode = capabilityStatementErrorCodeOrDetail.FirstOrDefault(x => x.SuppliedConsumerOdsCode == ConsumerOdsCodeAsList[i]);

            //            if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None)
            //            {
            //                var slotSearchErrorCodeOrDetail = GetSlotSearchErrorCodeOrDetail(ConsumerOdsCodeAsList[i], slotSearchSummaryList);
            //                organisationErrorCodeOrDetailForCode.errorSource = slotSearchErrorCodeOrDetail.Item1;
            //                organisationErrorCodeOrDetailForCode.details = AppendAdditionalDetails(slotSearchErrorCodeOrDetail.Item2, organisationErrorCodeOrDetailForCode.additionalDetails);
            //                slotCount = slotSearchErrorCodeOrDetail.Item3.GetValueOrDefault();
            //            }
            //            else if (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && capabilityStatementErrorCodeOrDetailForCode.errorSource != ErrorCode.None)
            //            {
            //                organisationErrorCodeOrDetailForCode.errorSource = capabilityStatementErrorCodeOrDetailForCode.errorSource;
            //                organisationErrorCodeOrDetailForCode.details = capabilityStatementErrorCodeOrDetailForCode.details;
            //                slotCount = 0;
            //            }

            //            _stopwatch.Stop();

            //            var searchResultToAdd = new SearchResult
            //            {
            //                SearchGroupId = createdSearchGroup.SearchGroupId,
            //                ProviderCode = ProviderOdsCodeAsList[0],
            //                ConsumerCode = ConsumerOdsCodeAsList[i],
            //                ConsumerOrganisationType = OrganisationTypes.FirstOrDefault(x => x.Value == consumerOrganisationType).Text,
            //                ErrorCode = (int)organisationErrorCodeOrDetailForCode.errorSource,
            //                Details = organisationErrorCodeOrDetailForCode.details,
            //                ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.ProductName,
            //                SearchDurationSeconds = _stopwatch.Elapsed.TotalSeconds,

            //            };

            //            _stopwatch.Reset();

            //            if (slotSearchSummaryList != null)
            //            {
            //                var spineMessageId = slotSearchSummaryList.FirstOrDefault(x => x.OdsCode == ConsumerOdsCodeAsList[i])?.SpineMessageId;
            //                searchResultToAdd.SpineMessageId = spineMessageId;
            //            }

            //            var searchResult = _applicationService.AddSearchResult(searchResultToAdd);

            //            slotEntrySummary.Add(new SlotEntrySummary
            //            {
            //                ProviderLocationName = organisationErrorCodeOrDetailForCode.providerOrganisation?.OrganisationLocation,
            //                ProviderOdsCode = ProviderOdsCodeAsList[0],
            //                ConsumerLocationName = organisationErrorCodeOrDetailForCode.consumerOrganisation?.OrganisationLocation,
            //                ConsumerOdsCode = ConsumerOdsCodeAsList[i],
            //                ConsumerOrganisationType = OrganisationTypes.FirstOrDefault(x => x.Value == consumerOrganisationType).Text,
            //                SearchSummaryDetail = organisationErrorCodeOrDetailForCode.details,
            //                ProviderPublisher = organisationErrorCodeOrDetailForCode.providerSpine?.ProductName,
            //                SearchResultId = searchResult.SearchResultId,
            //                DetailsEnabled = (organisationErrorCodeOrDetailForCode.errorSource == ErrorCode.None && slotCount > 0),
            //                DisplayProvider = organisationErrorCodeOrDetailForCode.providerOrganisation != null,
            //                DisplayConsumer = organisationErrorCodeOrDetailForCode.consumerOrganisation != null,
            //                DisplayClass = (organisationErrorCodeOrDetailForCode.errorSource != ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
            //            });
            //        }
            //    }
            //}

            //_applicationService.UpdateSearchGroup(SearchGroupId);

            return slotEntrySummary.OrderBy(x => x.SearchResultId).ToList();
        }

        private string AppendAdditionalDetails(string item2, string additionalDetails)
        {
            return !string.IsNullOrEmpty(additionalDetails) ? $"{item2}<div class=\"nhsuk-warning-message\"><p>{additionalDetails}</p></div>" : item2;
        }

        private (ErrorCode, string, int?) GetSlotSearchErrorCodeOrDetail(string providerOdsCode, List<SlotEntrySummaryCount> slotEntrySummaries)
        {
            var slotEntrySummary = slotEntrySummaries.FirstOrDefault(x => x.OdsCode == providerOdsCode);
            var errorSource = slotEntrySummary?.ErrorCode ?? ErrorCode.None;
            var detail = string.Empty;

            if (errorSource == ErrorCode.None || errorSource == ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement)
            {
                if (slotEntrySummary != null && slotEntrySummary.FreeSlotCount.GetValueOrDefault() > 0)
                {
                    detail = StringExtensions.Pluraliser(SearchConstants.SEARCHSTATSCOUNTTEXT, slotEntrySummary.FreeSlotCount.GetValueOrDefault());
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

            if (slotEntrySummary != null)
            {
                _logger.LogInformation(errorSource.ToString());
                _logger.LogInformation(detail);
                _logger.LogInformation(slotEntrySummary.ErrorCode.ToString());
                _logger.LogInformation(slotEntrySummary.FreeSlotCount.ToString());
            }
            else
            {
                _logger.LogWarning("slotEntrySummary is null");
                _logger.LogWarning(errorSource.ToString());
            }

            return (errorSource, detail, slotEntrySummary?.FreeSlotCount);
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
            List<SpineList> consumerGpConnectDetails, List<OrganisationList> consumerOrganisationDetails, string consumerOrganisationType = "")
        {
            //var providerOrganisationLookupErrors = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            //var providerGpConnectDetailsErrors = providerGpConnectDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            //var providerErrorCode = providerOrganisationLookupErrors ?? providerGpConnectDetailsErrors ?? ErrorCode.None;

            //var consumerErrorCode = ErrorCode.None;

            //if (!string.IsNullOrEmpty(consumerOdsCode))
            //{
            //    var consumerOrganisationLookupErrors = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            //    var consumerGpConnectDetailsErrors = consumerGpConnectDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode && x.ErrorCode != ErrorCode.None)?.ErrorCode;
            //    consumerErrorCode = consumerOrganisationLookupErrors ?? consumerGpConnectDetailsErrors ?? ErrorCode.None;
            //}

            //var errorSource = ErrorCode.None;
            //var details = string.Empty;
            //var additionalDetails = string.Empty;
            //Organisation providerOrganisation = null;
            //Organisation consumerOrganisation = null;
            //Spine providerSpine = null;

            //if (providerErrorCode == ErrorCode.None)
            //{
            //    providerOrganisation = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode)?.Organisation;
            //    providerSpine = providerGpConnectDetails.FirstOrDefault(x => x.OdsCode == providerOdsCode)?.Spine;
            //}
            //else
            //{
            //    errorSource = providerErrorCode;
            //    details = string.Format(errorSource.GetDescription(), providerOdsCode);
            //}

            //if (consumerErrorCode == ErrorCode.None)
            //{
            //    consumerOrganisation = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode)?.Organisation;
            //}
            //else if (consumerErrorCode == ErrorCode.ConsumerNotEnabledForGpConnectAppointmentManagement)
            //{
            //    consumerOrganisation = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerOdsCode)?.Organisation;
            //    additionalDetails = string.Format(consumerErrorCode.GetDescription(), consumerOdsCode);
            //}
            //else
            //{
            //    errorSource = consumerErrorCode;
            //    details = string.Format(errorSource.GetDescription(), consumerOdsCode);
            //}

            return new OrganisationErrorCodeOrDetail
            {
                //SuppliedProviderOdsCode = providerOdsCode,
                //SuppliedConsumerOdsCode = consumerOdsCode,
                //errorSource = errorSource,
                //details = details,
                //additionalDetails = additionalDetails,
                //providerOrganisation = providerOrganisation,
                //consumerOrganisation = consumerOrganisation,
                //providerSpine = providerSpine
            };
        }

        private IEnumerable<SelectListItem> GetDateRanges()
        {
            var dateRange = new List<SelectListItem>();
            var firstDayOfCurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            for (var i = 0; i < MaxNumberWeeksSearch; i++)
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

        private async Task<IEnumerable<SelectListItem>> GetOrganisationTypes()
        {
            var organisationTypes = await _configurationService.GetOrganisationTypes();
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
