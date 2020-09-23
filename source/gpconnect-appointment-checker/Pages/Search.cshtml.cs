using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using gpconnect_appointment_checker.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchModel : PageModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        public SearchResultItemList SearchResults { get; set; }

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

        public string[] ResultColumns { get; set; }

        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<SearchModel> _logger;
        protected ILdapService _ldapService;

        public SearchModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<SearchModel> logger, ILdapService ldapService)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _ldapService = ldapService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var providerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ProviderODSCode);
                var consumerOrganisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(ConsumerODSCode);

                var providerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndAsIdByOdsCode(ProviderODSCode);
                var consumerGpConnectDetails = await _ldapService.GetGpProviderEndpointAndAsIdByOdsCode(ConsumerODSCode);

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

                SearchResults = GetSearchResults();
                ResultColumns = new[]
                {
                    "Appointment Date", "Session Name", "Start Time", "Duration", "Slot Type", "Delivery Channel",
                    "Practitioner", "Practitioner Role", "Practitioner Gender"
                };
                return Page();
            }
            return Page();
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

        private SearchResultItemList GetSearchResults()
        {
            var results = new SearchResultItemList
            {
                new SearchResultItem()
                {
                    AppointmentDate = DateTime.Now.ToString("ddd d MMM yyyy"),
                    DeliveryChannel = "In Person",
                    Duration = 10.DurationFormatter("Mins"),
                    Location = "Laurel Bank Surgery, North Lane, Skipton",
                    Practitioner = "ROBERTS, Sam (Mr)",
                    PractitionerRole = "Nurse Practitioner",
                    PractitionerGender = "Male",
                    SessionName = "Nurse Clinic",
                    SlotType = "Child Immunisation",
                    StartTime = DateTime.Now.ToString("t")
                },
                new SearchResultItem()
                {
                    AppointmentDate = DateTime.Now.AddDays(2).ToString("ddd d MMM yyyy"),
                    DeliveryChannel = "In Person",
                    Duration = 10.DurationFormatter("Mins"),
                    Location = "Laurel Bank Surgery, North Lane, Skipton",
                    Practitioner = "JEFFERIES, Lisa (Mrs)",
                    PractitionerRole = "Nurse Practitioner",
                    PractitionerGender = "Female",
                    SessionName = "Nurse Clinic",
                    SlotType = "Adult Immunisation",
                    StartTime = DateTime.Now.AddDays(2).ToString("t")
                }
            };
            return results;
        }
    }
}
