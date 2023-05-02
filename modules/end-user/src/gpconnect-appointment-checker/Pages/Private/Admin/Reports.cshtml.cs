using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public partial class ReportsModel : BaseModel
    {
        protected ILogger<ReportsModel> _logger;
        protected IReportingService _reportingService;
        protected readonly ILoggerManager _loggerManager;

        public ReportsModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor, ILogger<ReportsModel> logger, IReportingService reportingService, ILoggerManager loggerManager = null) : base(configuration, contextAccessor)
        {
            _logger = logger;
            _reportingService = reportingService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public async Task OnGet()
        {
            await RefreshPage();
        }

        private async Task<IActionResult> RefreshPage()
        {
            var reportsList = new List<SelectListItem>
            {
                new SelectListItem(ReportConstants.SLOTSUMMARYREPORTDEFAULT, String.Empty)
            };
            
            var reports = await _reportingService.GetReports();

            reportsList.AddRange(reports.Select(r => new SelectListItem
            {
                Text = r.ReportName,
                Value = r.FunctionName
            }).ToList());

            ReportsList = reportsList;
            return Page();
        }

        public async Task OnPostLoadReport()
        {
            if (ModelState.IsValid)
            {
                var report = await _reportingService.GetReport(SelectedReport);
                //ReportData = report;
            }
        }

        public async Task<FileStreamResult> OnPostExportReport()
        {
            if (ModelState.IsValid)
            {
                var reportName = ReportsList.FirstOrDefault(x => x.Value == SelectedReport)?.Text;
                var memoryStream = await _reportingService.ExportReport(SelectedReport, reportName);
                //return GetFileStream(memoryStream, $"{SelectedReport}_{DateTime.UtcNow.ToFileTimeUtc()}.xlsx");
            }
            return null;
        }
    }
}
