using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel
    {
        public List<SelectListItem> ReportsList => GetReportsList();
        [BindProperty]
        public string SelectedReport { get; set; }

        public DataTable ReportData { get; set; }
    }
}
