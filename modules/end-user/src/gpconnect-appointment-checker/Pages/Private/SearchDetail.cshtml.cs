using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.Helpers.Constants;
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
        protected readonly ILoggerManager _loggerManager;
        private readonly IExportService _exportService;

        public SearchDetailModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor, ILogger<SearchDetailModel> logger, IExportService exportService, IApplicationService applicationService, ISearchService searchService, ILoggerManager loggerManager = null) : base(configuration, contextAccessor)
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
            _exportService = exportService;
        }

        public async Task<IActionResult> OnGet(int searchDetailId)
        {
            await GetSearchResults(searchDetailId);
            return Page();
        }

        private async Task GetSearchResults(int searchResultId)
        {
            var searchResult = await _applicationService.GetSearchResult(searchResultId);
            if (searchResult != null)
            {
                SearchAtResultsText = searchResult.SearchAtResults;
                SearchOnBehalfOfResultsText = searchResult.SearchOnBehalfOfResults;

                var searchResponse = await _searchService.ExecuteFreeSlotSearchFromDatabase(new GpConnect.AppointmentChecker.Models.Request.SearchRequestFromDatabase() { SearchResultId = searchResultId });

                SearchAtResultsText = searchResponse.FormattedProviderOrganisationDetails;
                SearchOnBehalfOfResultsText = GetSearchOnBehalfOfResultsText(searchResponse.FormattedConsumerOrganisationDetails, searchResponse.FormattedConsumerOrganisationType); ;
                ProviderPublisher = searchResponse.ProviderPublisher;

                SearchResultsTotalCount = searchResponse.SearchResultsTotalCount;
                SearchResultsCurrentCount = searchResponse.SearchResultsCurrentCount;
                SearchResultsPastCount = searchResponse.SearchResultsPastCount;

                SearchResultsCurrent = searchResponse.CurrentSlotEntriesByLocationGrouping;
                SearchResultsPast = searchResponse.PastSlotEntriesByLocationGrouping;

                SearchGroupId = searchResponse.SearchGroupId;
                SearchResultId = searchResponse.SearchResultId;
                SearchDuration = searchResponse.TimeTaken;
            }
        }

        public async Task<FileStreamResult> OnPostExportSearchResult(int searchResultId)
        {
            var filestream = await _exportService.ExportSearchResultFromDatabase(new GpConnect.AppointmentChecker.Models.Request.SearchExport()
            {
                ExportRequestId = searchResultId,
                UserId = UserId,
                ReportName = ReportConstants.Slotsummaryreportheading
            });
            return filestream;
        }
    }
}
