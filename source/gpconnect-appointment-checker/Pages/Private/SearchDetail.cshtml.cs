using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : PageModel
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchDetailModel> _logger;
        protected ILdapService _ldapService;
        protected IApplicationService _applicationService;
        protected ITokenService _tokenService;
        protected IGpConnectQueryExecutionService _queryExecutionService;
        protected IAuditService _auditService;
        protected readonly ILoggerManager _loggerManager;

        public SearchDetailModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchDetailModel> logger, ILdapService ldapService, ITokenService tokenService, IGpConnectQueryExecutionService queryExecutionService, IApplicationService applicationService, IAuditService auditService, ILoggerManager loggerManager = null)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _ldapService = ldapService;
            _tokenService = tokenService;
            _queryExecutionService = queryExecutionService;
            _applicationService = applicationService;
            _auditService = auditService;
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
                SearchOnBehalfOfResultsText = $"{searchResult.ConsumerOrganisationName} ({searchResult.ConsumerOdsCode}) - {StringExtensions.AddressBuilder(searchResult.ConsumerAddressFields.ToList(), searchResult.ConsumerPostcode)}";
                SearchResults = new List<List<SlotEntrySimple>>();
                SearchGroupId = searchResult.SearchGroupId;
                SearchResultId = searchResult.SearchResultId;
                ProviderPublisher = searchResult.ProviderPublisher;
                SearchDuration = searchResult.SearchDurationSeconds;

                var searchResults = _queryExecutionService.ExecuteFreeSlotSearchFromDatabase(searchResult.ResponsePayload);
                var locationGrouping = searchResults?.SlotEntrySimple.GroupBy(l => l.LocationName)
                    .Select(grp => grp.ToList()).ToList();
                
                SearchResultsCount = searchResults?.SlotEntrySimple.Count;
                if (locationGrouping != null)
                {
                    SearchResults.AddRange(locationGrouping);
                }
            }
        }
    }
}
