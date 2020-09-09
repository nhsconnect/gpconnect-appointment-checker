using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.ViewModels.Search
{
    [Serializable]
    public class SearchForm
    {
        public List<SelectListItem> DateRanges { get; set; }

        public string ProviderODSCode { get; set; }
        public string ConsumerODSCode { get; set; }
        public string SelectedDateRange { get; set; }

        public string SearchButtonText { get; set; }
        public string ClearButtonText { get; set; }

        public string SearchAtText { get; set; }
        public string SearchOnBehalfOfText { get; set; }
        public string DateRangeText { get; set; }
        public string SearchResultsHeadingText { get; set; }

        public string[] ResultColumns { get; set; }

        public SearchResultItemList SearchResults { get; set; }
    }
}
