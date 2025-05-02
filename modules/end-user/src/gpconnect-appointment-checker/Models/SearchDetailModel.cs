using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchDetailModel : SearchBaseModel
    {
        public string SearchStats => string.Format(SearchConstants.SearchStatsText, SearchDuration.ToString("#.##s"),
            DateTime.Now.TimeZoneConverter("Europe/London", "d MMM yyyy HH:mm:ss"));
    }
}
