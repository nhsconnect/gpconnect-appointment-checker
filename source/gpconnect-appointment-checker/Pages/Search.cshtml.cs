using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchModel : PageModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        public List<SlotSimple> SearchResults { get; set; }

        [Required]
        [BindProperty]
        public string ProviderODSCode { get; set; }
        
        [Required]
        [BindProperty]
        public string ConsumerODSCode { get; set; }

        [BindProperty]
        public string SearchAtResultsText { get; set; }
        [BindProperty]
        public string SearchOnBehalfOfResultsText { get; set; }
        [BindProperty] 
        public string SelectedDateRange { get; set; }

        public double SearchDuration { get; set; }

        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchModel> _logger;
        protected ILdapService _ldapService;
        protected ITokenService _tokenService;
        protected IGPConnectQueryExecutionService _queryExecutionService;

        public SearchModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ILdapService ldapService, ITokenService tokenService, IGPConnectQueryExecutionService queryExecutionService)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _ldapService = ldapService;
            _tokenService = tokenService;
            _queryExecutionService = queryExecutionService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var searchTimer = new Stopwatch();
                searchTimer.Start();
                await GetSearchResults();
                searchTimer.Stop();
                SearchDuration = searchTimer.Elapsed.TotalSeconds;
            }
            return Page();
        }

        private async Task GetSearchResults()
        {
            var providerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ProviderODSCode);
            var consumerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ConsumerODSCode);

            var providerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndAsIdByOdsCode(ProviderODSCode);
            var consumerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndAsIdByOdsCode(ConsumerODSCode);

            await PopulateSearchResults(providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);

            if (providerOrganisationDetails != null)
            {
                SearchAtResultsText =
                    $"{providerOrganisationDetails.OrganisationName} ({providerOrganisationDetails.ODSCode}) - {providerOrganisationDetails.PostalAddress} {providerOrganisationDetails.PostalCode}";
            }

            if (consumerOrganisationDetails != null)
            {
                SearchOnBehalfOfResultsText =
                    $"{consumerOrganisationDetails.OrganisationName} ({consumerOrganisationDetails.ODSCode}) - {consumerOrganisationDetails.PostalAddress} {consumerOrganisationDetails.PostalCode}";
            }
        }

        private async Task PopulateSearchResults(Spine providerGpConnectDetails, Organisation providerOrganisationDetails,
            Spine consumerGpConnectDetails, Organisation consumerOrganisationDetails)
        {
            var requestParameters = await _tokenService.ConstructRequestParameters(
                _contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerGpConnectDetails, consumerOrganisationDetails);
            if (requestParameters != null)
            {
                var searchResults = await _queryExecutionService.ExecuteFreeSlotSearch(requestParameters, DateTime.Today,
                    DateTime.Today.AddDays(7), providerGpConnectDetails.SSPHostname);
                SearchResults = searchResults;
            }
        }

        public IActionResult OnPostClear()
        {
            return RedirectToPage("Search");
        }

        private List<SelectListItem> GetDateRanges()
        {
            _logger.LogInformation("Getting DateRanges");

            var weeksToGet = _configuration["MaxNumberOfWeeks"].StringToInteger(12);
            var dateRange = new List<SelectListItem>();
            var firstDayOfCurrentWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            for (var i = 0; i < weeksToGet; i++)
            {
                var week = new SelectListItem
                {
                    Text = $"{firstDayOfCurrentWeek:ddd d-MMM} - {firstDayOfCurrentWeek.AddDays(6):ddd d-MMM}",
                    Value = $"{firstDayOfCurrentWeek:d-MMM-yyyy}:{firstDayOfCurrentWeek.AddDays(6):d-MMM-yyyy}"
                };
                dateRange.Add(week);
                firstDayOfCurrentWeek = firstDayOfCurrentWeek.AddDays(7);
            }
            return dateRange;
        }
    }
}
