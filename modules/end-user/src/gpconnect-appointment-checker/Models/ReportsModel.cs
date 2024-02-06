using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel
    {
        public IEnumerable<SelectListItem> ReportsList => GetReportsList().Result;

        [BindProperty]
        public string? SelectedReport { get; set; }

        public DataTable ReportData { get; set; }
    }
}
