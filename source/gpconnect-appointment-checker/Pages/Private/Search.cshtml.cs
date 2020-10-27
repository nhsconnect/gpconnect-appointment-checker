using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
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
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchModel : PageModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        public List<SlotEntrySimple> SearchResults { get; set; } = null;

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
        public bool ProviderODSCodeFound { get; set; } = true;
        public bool ConsumerODSCodeFound { get; set; } = true;

        public bool ProviderASIDPresent { get; set; } = true;
        public bool ProviderEnabledForGpConnectAppointmentManagement { get; set; } = true;

        public bool SlotSearchOk { get; set; } = true;
        public bool CapabilityStatementOk { get; set; } = true;
        public string ProviderErrorDisplay { get; set; }
        public string ProviderErrorCode { get; set; }
        public string ProviderErrorDiagnostics { get; set; }

        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchModel> _logger;
        protected ILdapService _ldapService;
        protected IApplicationService _applicationService;
        protected ITokenService _tokenService;
        protected IGPConnectQueryExecutionService _queryExecutionService;

        public SearchModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ILdapService ldapService, ITokenService tokenService, IGPConnectQueryExecutionService queryExecutionService, IApplicationService applicationService)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _ldapService = ldapService;
            _tokenService = tokenService;
           _queryExecutionService = queryExecutionService;
           _applicationService = applicationService;
        }

        public IActionResult OnGet()
        {
            var userCode = User.GetClaimValue("ProviderODSCode");
            if (!string.IsNullOrEmpty(userCode)) ProviderODSCode = userCode;
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

        public IActionResult OnPostClear()
        {
            return RedirectToPage("Search");
        }


        private async Task GetSearchResults()
        {
            var providerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ProviderODSCode);
            var consumerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ConsumerODSCode);

            ProviderODSCodeFound = providerOrganisationDetails != null;
            ConsumerODSCodeFound = consumerOrganisationDetails != null;

            if (ProviderODSCodeFound && ConsumerODSCodeFound)
            {
                _applicationService.SynchroniseOrganisation(providerOrganisationDetails);
                _applicationService.SynchroniseOrganisation(consumerOrganisationDetails);

                //Step 2 - VALIDATE PROVIDER ODS CODE IN SPINE DIRECTORY
                //Is ODS code configured in Spine Directory as an GP Connect Appointments provider system? / Retrieve provider endpoint and party key from Spine Directory
                var providerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ProviderODSCode);
                var consumerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(ConsumerODSCode);
                ProviderEnabledForGpConnectAppointmentManagement = providerGpConnectDetails != null;

                if (ProviderEnabledForGpConnectAppointmentManagement)
                {
                    var providerAsId = await _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(ProviderODSCode, providerGpConnectDetails.party_key);
                    ProviderASIDPresent = providerAsId != null;

                    if (ProviderASIDPresent)
                    {
                        providerGpConnectDetails.asid = providerAsId.asid;
                        await PopulateSearchResults(providerGpConnectDetails, providerOrganisationDetails, consumerGpConnectDetails, consumerOrganisationDetails);
                        SearchAtResultsText = $"{providerOrganisationDetails.OrganisationName} ({providerOrganisationDetails.ODSCode}) - {providerOrganisationDetails.PostalAddress} {providerOrganisationDetails.PostalCode}";
                        SearchOnBehalfOfResultsText = $"{consumerOrganisationDetails.OrganisationName} ({consumerOrganisationDetails.ODSCode}) - {consumerOrganisationDetails.PostalAddress} {consumerOrganisationDetails.PostalCode}";
                    }
                }
            }
        }

        private async Task PopulateSearchResults(Spine providerGpConnectDetails, Organisation providerOrganisationDetails,
            Spine consumerGpConnectDetails, Organisation consumerOrganisationDetails)
        {
            var requestParameters = await _tokenService.ConstructRequestParameters(
                _contextAccessor.HttpContext.GetAbsoluteUri(), providerGpConnectDetails, providerOrganisationDetails,
                consumerGpConnectDetails, consumerOrganisationDetails, (int)SpineMessageTypes.GpConnectSearchFreeSlots);

            if (requestParameters != null)
            {
                //Step 3 - CALL PROVIDER METADATA ENDPOINT
                //Get capability statement
                var capabilityStatement = await _queryExecutionService.ExecuteFhirCapabilityStatement(requestParameters, providerGpConnectDetails.ssp_hostname);
                CapabilityStatementOk = capabilityStatement.Issue == null;

                if (CapabilityStatementOk)
                {
                    var startDate = Convert.ToDateTime(SelectedDateRange.Split(":")[0]);
                    var endDate = Convert.ToDateTime(SelectedDateRange.Split(":")[1]);
                    var searchResults = await _queryExecutionService.ExecuteFreeSlotSearch(requestParameters, startDate, endDate, providerGpConnectDetails.ssp_hostname);
                    SlotSearchOk = searchResults.Issue == null;

                    if (SlotSearchOk)
                    {
                        SearchResults = searchResults.SlotEntrySimple;
                    }
                    else
                    {
                        ProviderErrorDisplay = searchResults.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
                        ProviderErrorCode = searchResults.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
                        ProviderErrorDiagnostics = searchResults.Issue.FirstOrDefault()?.Diagnostics;
                    }
                }
                else
                {
                    ProviderErrorDisplay = capabilityStatement.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Display;
                    ProviderErrorCode = capabilityStatement.Issue.FirstOrDefault()?.Details.Coding.FirstOrDefault()?.Code;
                    ProviderErrorDiagnostics = capabilityStatement.Issue.FirstOrDefault()?.Diagnostics;
                }
            }
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
