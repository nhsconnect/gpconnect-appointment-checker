using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel
    {
        public IEnumerable<SelectListItem> ReportsList => GetReportsList().Result;
        public IEnumerable<SelectListItem> CapabilityReportsList => GetCapabilityReportsList().Result;        

        [BindProperty]
        public string? SelectedReport { get; set; }

        [BindProperty]
        public string? SelectedCapabilityReport { get; set; }        

        [BindProperty]
        [Display(Name = ReportConstants.ODSCODES)]
        public string? OdsCodes { get; set; }

        public List<string>? OdsCodeList => OdsCodes?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToString().ToUpper()).ToList();

        public DataTable ReportData { get; set; }
    }
}
