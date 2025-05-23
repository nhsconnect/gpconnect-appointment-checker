using gpconnect_appointment_checker.Helpers.Constants;
using System;
using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : SearchBaseModel
    {
        public string SearchStats => string.Format(SearchConstants.SearchStatsText, SearchDuration.ToString("#.##s"),
            DateTime.Now.TimeZoneConverter("Europe/London", "d MMM yyyy HH:mm:ss"));
    }
}
