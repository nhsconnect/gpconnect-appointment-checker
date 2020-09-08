using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.ViewModels.Search
{
    [Serializable]
    public class SearchForm
    {
        public List<SelectListItem> ProviderODSCodes { get; set; }
        public List<SelectListItem> ConsumerODSCodes { get; set; }
        public List<SelectListItem> DateRanges { get; set; }

        public string SelectedProviderODSCode { get; set; }
        public string SelectedConsumerODSCode { get; set; }
        public string SelectedDateRange { get; set; }

        public string SearchButtonText { get; set; }
        public string ClearButtonText { get; set; }
    }
}
