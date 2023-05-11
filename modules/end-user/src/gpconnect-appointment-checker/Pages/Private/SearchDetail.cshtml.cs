using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : SearchBaseModel
    {
        protected IOptions<GeneralConfig> _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchDetailModel> _logger;
        protected IApplicationService _applicationService;
        protected ISearchService _searchService;
        protected IReportingService _reportingService;
        protected readonly ILoggerManager _loggerManager;

        public SearchDetailModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor, ILogger<SearchDetailModel> logger, IApplicationService applicationService, IReportingService reportingService, ISearchService searchService, ILoggerManager loggerManager = null) : base(configuration, contextAccessor, reportingService)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _applicationService = applicationService;
            _searchService = searchService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public async Task<IActionResult> OnGet(int searchDetailId)
        {
            await GetSearchResults(searchDetailId);
            return Page();
        }

        private async Task GetSearchResults(int searchResultId)
        {
            var searchResult = await _applicationService.GetSearchResult(searchResultId, UserId);
            if (searchResult != null)
            {
                SearchAtResultsText = searchResult.SearchAtResults;
                SearchOnBehalfOfResultsText = searchResult.SearchOnBehalfOfResults;

                var searchResponse = await _searchService.ExecuteFreeSlotSearchFromDatabase(new GpConnect.AppointmentChecker.Models.Request.SearchRequestFromDatabase() { UserId = UserId, SearchResultId = searchResultId });

                SearchAtResultsText = searchResponse.FormattedProviderOrganisationDetails;
                SearchOnBehalfOfResultsText = searchResponse.FormattedConsumerOrganisationDetails;
                ProviderPublisher = searchResponse.ProviderPublisher;

                SearchResultsTotalCount = searchResponse.SearchResultsTotalCount;
                SearchResultsCurrentCount = searchResponse.SearchResultsCurrentCount;
                SearchResultsPastCount = searchResponse.SearchResultsPastCount;

                SearchResultsCurrent = searchResponse.CurrentSlotEntriesByLocationGrouping;
                SearchResultsPast = searchResponse.PastSlotEntriesByLocationGrouping;

                //SearchExportId = searchResults.SearchExportId;

                SearchGroupId = searchResult.SearchGroupId;
                SearchResultId = searchResult.SearchResultId;
                ProviderPublisher = searchResult.ProviderPublisher;
                SearchDuration = searchResult.SearchDurationSeconds;
            }
        }

        public async Task<FileStreamResult> OnPostExportSearchResults(int searchexportid)
        {
            var exportTable = await _applicationService.GetSearchExport(searchexportid, UserId);
            //return ExportResult(exportTable);
            return null;
        }
    }
}
