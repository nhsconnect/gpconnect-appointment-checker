using gpconnect_appointment_checker.DTO.Response.GpConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        public List<List<SlotEntrySimple>> SearchResults { get; set; }

        [Required(ErrorMessage = "You must enter a provider ODS code")]
        [BindProperty]
        public string ProviderODSCode { get; set; }

        [Required(ErrorMessage = "You must enter a consumer ODS code")]
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
        public int? SearchResultsCount { get; set; }
        public bool LdapErrorRaised { get; set; }
    }
}
