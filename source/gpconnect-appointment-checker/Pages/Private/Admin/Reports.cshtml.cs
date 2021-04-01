using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel : PageModel
    {
        protected ILogger<ReportsModel> _logger;
        protected IReportingService _reportingService;
        protected readonly ILoggerManager _loggerManager;

        public ReportsModel(ILogger<ReportsModel> logger, IReportingService reportingService, ILoggerManager loggerManager = null)
        {
            _logger = logger;
            _reportingService = reportingService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public void OnGet()
        {
        }

        public void OnPostLoadReport()
        {
            var report = _reportingService.GetReport(SelectedReport);
            ReportData = report;
        }

        public void OnPostExportReport()
        {
            _reportingService.ExportReport(SelectedReport);
        }

        private List<SelectListItem> GetReportsList()
        {
            var reportList = _reportingService.GetReports().Select(r => new SelectListItem
            {
                Text = r.ReportName,
                Value = r.FunctionName
            }).ToList();
            return reportList;
        }
    }
}
