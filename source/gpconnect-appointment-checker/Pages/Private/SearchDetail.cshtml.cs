using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : SearchBaseModel
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchDetailModel> _logger;
        protected ILdapService _ldapService;
        protected IApplicationService _applicationService;
        protected ITokenService _tokenService;
        protected IGpConnectQueryExecutionService _queryExecutionService;
        protected IAuditService _auditService;
        protected IReportingService _reportingService;
        protected readonly ILoggerManager _loggerManager;

        public SearchDetailModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchDetailModel> logger, ILdapService ldapService, ITokenService tokenService, IGpConnectQueryExecutionService queryExecutionService, IApplicationService applicationService, IAuditService auditService, IReportingService reportingService, ILoggerManager loggerManager = null) : base(applicationService, reportingService)
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
        }

        public IActionResult OnGet(int searchDetailId)
        {
            GetSearchResults(searchDetailId);
            return Page();
        }

        private void GetSearchResults(int searchResultId)
        {
            var userId = User.GetClaimValue("UserId").StringToInteger();
            var searchResult = _applicationService.GetSearchResult(searchResultId, userId);
            if (searchResult != null)
            {
                SearchAtResultsText = $"{searchResult.ProviderOrganisationName} ({searchResult.ProviderOdsCode}) - {StringExtensions.AddressBuilder(searchResult.ProviderAddressFields.ToList(), searchResult.ProviderPostcode)}";
                SearchOnBehalfOfResultsText = GetSearchOnBehalfOfResultsText(searchResult);

                var searchResults = _queryExecutionService.ExecuteFreeSlotSearchFromDatabase(searchResult.ResponsePayload, userId);

                SearchExportId = searchResults.SearchExportId;
                SearchResults = new List<List<SlotEntrySimple>>();
                SearchResultsPast = new List<List<SlotEntrySimple>>();

                SearchGroupId = searchResult.SearchGroupId;
                SearchResultId = searchResult.SearchResultId;
                ProviderPublisher = searchResult.ProviderPublisher;
                SearchDuration = searchResult.SearchDurationSeconds;

                SearchResultsTotalCount = searchResults.SlotCount;
                SearchResultsCurrentCount = searchResults.CurrentSlotCount;
                SearchResultsPastCount = searchResults.PastSlotCount;

                SearchResults.AddRange(searchResults.CurrentSlotEntriesByLocationGrouping);
                SearchResultsPast.AddRange(searchResults.PastSlotEntriesByLocationGrouping);
            }
        }

        public FileStreamResult OnPostExportSearchResults(int searchexportid)
        {
            return ExportSearchResults(searchexportid);
        }

        private string GetSearchOnBehalfOfResultsText(SearchResult searchResult)
        {
            var searchOnBehalfOfResultsText = new StringBuilder();

            if (!string.IsNullOrEmpty(searchResult.ConsumerOrganisationName))
            {
                searchOnBehalfOfResultsText.Append($"{searchResult.ConsumerOrganisationName} ({searchResult.ConsumerOdsCode}) - {StringExtensions.AddressBuilder(searchResult.ConsumerAddressFields.ToList(), searchResult.ConsumerPostcode)}");
            }
            if (!string.IsNullOrEmpty(searchResult.ConsumerOrganisationType))
            {
                searchOnBehalfOfResultsText.Append($"<p>{string.Format(SearchConstants.SEARCHRESULTSSEARCHONBEHALFOFORGTYPETEXT, searchResult.ConsumerOrganisationType)}</p>");
            }
            return searchOnBehalfOfResultsText.ToString();
        }
    }
}
