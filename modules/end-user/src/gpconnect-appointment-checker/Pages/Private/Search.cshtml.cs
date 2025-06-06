﻿using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

using SlotEntrySummary = GpConnect.AppointmentChecker.Models.SlotEntrySummary;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel : SearchBaseModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IExportService _exportService;
        private readonly IConfigurationService _configurationService;
        private readonly ISearchService _searchService;

        public SearchModel(IOptions<GeneralConfig> configuration,
            IHttpContextAccessor contextAccessor,
            ILogger<SearchModel> logger,
            IExportService exportService,
            IApplicationService applicationService,
            IConfigurationService configurationService,
            ISearchService searchService) : base(configuration, contextAccessor)
        {
            _applicationService = applicationService;
            _configurationService = configurationService;
            _searchService = searchService;
            _exportService = exportService;
        }

        public async Task<IActionResult> OnGet(string providerOdsCode, string consumerOdsCode)
        {
            if (!string.IsNullOrEmpty(providerOdsCode) && !string.IsNullOrEmpty(consumerOdsCode))
            {
                ProviderOdsCode = providerOdsCode;
                ConsumerOdsCode = consumerOdsCode;
            }
            else
            {
                ProviderOdsCode = string.Empty;
                ConsumerOdsCode = string.Empty;
            }

            //OrganisationTypes = await GetOrganisationTypes();

            ModelState.ClearValidationState("ProviderOdsCode");
            ModelState.ClearValidationState("ConsumerOdsCode");

            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            CheckInputs();
            if (!ModelState.IsValid) return Page();

            ProviderOdsCode = CleansedProviderOdsCodeInput;
            ConsumerOdsCode = CleansedConsumerOdsCodeInput;
            await GetSearchResults();

            return Page();
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

        public async Task<IActionResult> OnGetSearchByGroup(int searchGroupId)
        {
            var searchGroup = await _applicationService.GetSearchGroup(searchGroupId);
            if (searchGroup != null)
            {
                ProviderOdsCode = searchGroup.ProviderOdsTextbox;
                ConsumerOdsCode = searchGroup.ConsumerOdsTextbox;
                SelectedDateRange = searchGroup.SelectedDateRange;
                SelectedOrganisationType = searchGroup.ConsumerOrganisationTypeDropdown;
                await PopulateSearchResultsForGroup(searchGroupId);
            }

            ModelState.ClearValidationState("ProviderOdsCode");
            ModelState.ClearValidationState("ConsumerOdsCode");
            ModelState.ClearValidationState("SelectedOrganisationType");
            return Page();
        }

        public async Task<FileStreamResult> OnPostExportSearchResult(int searchResultId)
        {
            var filestream = await _exportService.ExportSearchResultFromDatabase(
                new GpConnect.AppointmentChecker.Models.Request.SearchExport()
                {
                    ExportRequestId = searchResultId,
                    UserId = UserId,
                    ReportName = ReportConstants.SlotSummaryReportHeading
                });
            return filestream;
        }

        public async Task<FileStreamResult> OnPostExportSearchGroup(int searchGroupId)
        {
            var filestream = await _exportService.ExportSearchGroupFromDatabase(
                new GpConnect.AppointmentChecker.Models.Request.SearchExport()
                {
                    ExportRequestId = searchGroupId,
                    UserId = UserId,
                    ReportName = ReportConstants.SlotSummaryReportHeading
                });
            return filestream;
        }

        private async Task PopulateSearchResultsForGroup(int searchGroupId)
        {
            var searchResultsForGroup = await _applicationService.GetSearchResultByGroup(searchGroupId);
            IsMultiSearch = true;

            var slotEntrySummaryList = new List<SlotEntrySummary>();

            slotEntrySummaryList.AddRange(searchResultsForGroup.Select(x => new SlotEntrySummary()
            {
                DisplayProvider = x.DisplayProvider,
                FormattedProviderOrganisationDetails = x.FormattedProviderOrganisationDetails,
                ProviderPublisher = x.ProviderPublisher,
                DisplayConsumer = x.DisplayConsumer,
                FormattedConsumerOrganisationDetails = x.FormattedConsumerOrganisationDetails,
                DisplayConsumerOrganisationType = x.DisplayConsumerOrganisationType,
                ConsumerOrganisationType = x.FormattedConsumerOrganisationType,
                DetailsEnabled = x.DetailsEnabled,
                ProviderOdsCode = x.ProviderOdsCode,
                ConsumerOdsCode = x.ConsumerOdsCode,
                SearchGroupId = x.SearchGroupId,
                SearchResultId = x.SearchResultId,
                SearchSummaryDetail = JsonConvert.DeserializeObject<List<string>>(x.DisplayDetails),
                DisplayClass = x.DisplayClass
            }));
            SearchResultsSummary = slotEntrySummaryList;
        }

        private void CheckInputs()
        {
            // check the following rules:
            if (!ValidSearchCombination)
            {
                ModelState.AddModelError("ProviderOdsCode", SearchConstants.IssueWithOdsCodesInputText);
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.IssueWithOdsCodesInputText);
            }

            if (OrgTypeSearchEnabled && (string.IsNullOrEmpty(ConsumerOdsCode) || ConsumerOdsCodeAsList?.Count == 0) &&
                string.IsNullOrEmpty(SelectedOrganisationType))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.ConsumerOdsCodeNotEnteredErrorMessage);
                ModelState.AddModelError("SelectedOrganisationType",
                    SearchConstants.ConsumerOrgTypeNotEnteredErrorMessage);
            }

            if (!OrgTypeSearchEnabled && (string.IsNullOrEmpty(ConsumerOdsCode) || ConsumerOdsCodeAsList?.Count == 0))
            {
                ModelState.AddModelError("ConsumerOdsCode", SearchConstants.ConsumerOdsCodeRequiredErrorMessage);
            }

            if ((string.IsNullOrEmpty(ProviderOdsCode) || ProviderOdsCodeAsList?.Count == 0))
            {
                ModelState.AddModelError("ProviderOdsCode", SearchConstants.ProviderOdsCodeRequiredErrorMessage);
            }
        }

        private async Task GetSearchResults()
        {
            var response = await _searchService.ExecuteSearch(
                new GpConnect.AppointmentChecker.Models.Request.SearchRequest()
                {
                    ProviderOdsCode = ProviderOdsCode,
                    ConsumerOdsCode = ConsumerOdsCode,
                    ConsumerOrganisationType = SelectedOrganisationType,
                    DateRange = SelectedDateRange,
                    RequestUri = FullUrl,
                    UserId = UserId,
                    UserSessionId = UserSessionId,
                    Sid = Sid
                });

            IsMultiSearch = response.Count > 1;

            if (!IsMultiSearch)
            {
                var searchResponse = response.FirstOrDefault();

                ProviderODSCodeFound = searchResponse.ProviderOdsCodeFound;
                ConsumerODSCodeFound = searchResponse.ConsumerOdsCodeFound;
                ProviderEnabledForGpConnectAppointmentManagement =
                    searchResponse.ProviderEnabledForGpConnectAppointmentManagement;
                ConsumerEnabledForGpConnectAppointmentManagement =
                    searchResponse.ConsumerEnabledForGpConnectAppointmentManagement;
                ProviderASIDPresent = searchResponse.ProviderASIDPresent;
                SearchAtResultsText = searchResponse.FormattedProviderOrganisationDetails;
                SearchOnBehalfOfResultsText = GetSearchOnBehalfOfResultsText(
                    searchResponse.FormattedConsumerOrganisationDetails,
                    searchResponse.FormattedConsumerOrganisationType);
                ProviderPublisher = searchResponse.ProviderPublisher;

                SearchResultsTotalCount = searchResponse.SearchResultsTotalCount;
                SearchResultsCurrentCount = searchResponse.SearchResultsCurrentCount;
                SearchResultsPastCount = searchResponse.SearchResultsPastCount;

                SearchResultsCurrent = searchResponse.CurrentSlotEntriesByLocationGrouping;
                SearchResultsPast = searchResponse.PastSlotEntriesByLocationGrouping;

                SearchGroupId = searchResponse.SearchGroupId;
                SearchResultId = searchResponse.SearchResultId;

                if (searchResponse.ProviderError != null)
                {
                    ProviderError = searchResponse.ProviderError;
                }
            }
            else
            {
                var slotEntrySummaryList = new List<SlotEntrySummary>();

                slotEntrySummaryList.AddRange(response.Select(x => new SlotEntrySummary()
                {
                    DisplayProvider = x.DisplayProvider,
                    FormattedProviderOrganisationDetails = x.FormattedProviderOrganisationDetails,
                    ProviderPublisher = x.ProviderPublisher,
                    DisplayConsumer = x.DisplayConsumer,
                    FormattedConsumerOrganisationDetails = x.FormattedConsumerOrganisationDetails,
                    DisplayConsumerOrganisationType = x.DisplayConsumerOrganisationType,
                    ConsumerOrganisationType = x.FormattedConsumerOrganisationType,
                    DetailsEnabled = x.DetailsEnabled,
                    ProviderOdsCode = x.ProviderOdsCode,
                    ConsumerOdsCode = x.ConsumerOdsCode,
                    SearchGroupId = x.SearchGroupId,
                    SearchResultId = x.SearchResultId,
                    SearchSummaryDetail = JsonConvert.DeserializeObject<List<string>>(x.DisplayDetails),
                    DisplayClass = x.DisplayClass
                }));
                SearchResultsSummary = slotEntrySummaryList;
                SearchGroupId = slotEntrySummaryList[0].SearchGroupId;
            }
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
            options.Insert(0, new SelectListItem()
            {
                Text = "N/A",
                Value = string.Empty
            });
            return options;
        }
    }
}
