using gpconnect_appointment_checker.DTO.Response.GpConnect;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel
    {
        public List<List<SlotEntrySimple>> SearchResults { get; set; }
        public List<List<SlotEntrySimple>> SearchResultsPast { get; set; }

        [BindProperty]
        public string SearchAtResultsText { get; set; }
        [BindProperty]
        public string SearchOnBehalfOfResultsText { get; set; }
        
        [BindProperty(Name = "SearchGroupId", SupportsGet = true)]
        public int SearchGroupId { get; set; }

        [BindProperty(Name = "SearchResultId", SupportsGet = true)]
        public int SearchResultId { get; set; }

        public double SearchDuration { get; set; }
        public int? SearchResultsCount { get; set; }
        public string ProviderPublisher { get; set; }
    }
}