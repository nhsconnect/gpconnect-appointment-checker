using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : SearchBaseModel
    {
        public List<List<SearchResultEntry>> SearchResultsCurrent { get; set; }
        public List<List<SearchResultEntry>> SearchResultsPast { get; set; }

        [BindProperty]
        public string SearchAtResultsText { get; set; }
        [BindProperty]
        public string SearchOnBehalfOfResultsText { get; set; }
        
        [BindProperty(Name = "SearchGroupId", SupportsGet = true)]
        public int SearchGroupId { get; set; }

        [BindProperty(Name = "SearchResultId", SupportsGet = true)]
        public int SearchResultId { get; set; }

        //[BindProperty(Name = "SearchExportId", SupportsGet = true)]
        //public int SearchExportId { get; set; }
        public double SearchDuration { get; set; }

        public string SearchStats => string.Format(SearchConstants.Searchstatstext, SearchDuration.ToString("#.##s"), DateTime.Now.TimeZoneConverter("Europe/London", "d MMM yyyy HH:mm:ss"));

        public int SearchResultsTotalCount { get; set; }
        public int SearchResultsCurrentCount { get; set; }
        public int SearchResultsPastCount { get; set; }

        public string ProviderPublisher { get; set; }
    }
}