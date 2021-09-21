using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel : BaseModel
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
            if (ModelState.IsValid)
            {
                var report = _reportingService.GetReport(SelectedReport);
                ReportData = report;
            }
        }

        public FileStreamResult OnPostExportReport()
        {
            if (ModelState.IsValid)
            {
                var reportName = ReportsList.FirstOrDefault(x => x.Value == SelectedReport)?.Text;
                var memoryStream = _reportingService.ExportReport(SelectedReport, reportName);
                return GetFileStream(memoryStream, $"{SelectedReport}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx");
            }
            return null;
        }

        private List<SelectListItem> GetReportsList()
        {
            var reportList = new List<SelectListItem>
            {
                new SelectListItem(ReportConstants.SLOTSUMMARYREPORTDEFAULT, String.Empty)
            };
            reportList.AddRange(_reportingService.GetReports().Select(r => new SelectListItem
            {
                Text = r.ReportName,
                Value = r.FunctionName
            }).ToList());
            return reportList;
        }
    }
}
