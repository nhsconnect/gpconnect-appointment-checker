﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel
    {
        public IEnumerable<SelectListItem> ReportsList => GetReportsList().Result;

        [BindProperty]
        [Required(ErrorMessage = ReportConstants.SLOTSUMMARYREPORTSELECTIONERROR)]
        public string SelectedReport { get; set; }

        public DataTable ReportData { get; set; }
    }
}
