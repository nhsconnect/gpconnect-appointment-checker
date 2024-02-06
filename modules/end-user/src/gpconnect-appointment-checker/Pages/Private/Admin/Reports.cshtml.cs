using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices;
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

        public async Task<IActionResult> OnGet()
        {
            return Page();
        }

        public async Task OnPostLoadReport()
        {
            if (!string.IsNullOrWhiteSpace(SelectedReport))
            {
                var report = await _reportingService.GetReport(SelectedReport);
                ReportData = report;
            }
        }

        public async Task<FileStreamResult> OnPostLoadCapabilityReport()
        {
            if(!string.IsNullOrWhiteSpace(OdsCodes) && !string.IsNullOrWhiteSpace(SelectedCapabilityReport))
            {
                var filestream = await _reportingService.ExportReport(new GpConnect.AppointmentChecker.Models.Request.ReportExport()
                {
                    OdsCodes = OdsCodeList,
                    InteractionId = SelectedCapabilityReport,
                    ReportName = CapabilityReportsList.FirstOrDefault(x => x.Value == SelectedCapabilityReport).Text
                });
                return filestream;
            }
            return null;
        }
        

        public async Task<FileStreamResult> OnPostExportReport()
        {
            if (ModelState.IsValid)
            {
                var filestream = await _reportingService.ExportReport(new GpConnect.AppointmentChecker.Models.Request.ReportExport()
                {
                    FunctionName = SelectedReport,
                    ReportName = ReportsList.FirstOrDefault(x => x.Value == SelectedReport).Text
                });
                return filestream;
            }
            return null;
        }

        private async Task<IEnumerable<SelectListItem>> GetReportsList()
        {            
            var reports = await _reportingService.GetReports();
            var options = reports.Select(ot => new SelectListItem()
            {
                Text = ot.ReportName,
                Value = ot.FunctionName
            }).ToList();
            options.Insert(0, new SelectListItem() 
            {
                Text = "Please select a report",
                Value = ""
            });
            return options;
        }

        private async Task<IEnumerable<SelectListItem>> GetCapabilityReportsList()
        {
            var reports = await _reportingService.GetCapabilityReports();
            var options = reports.Select(ot => new SelectListItem()
            {
                Text = $"{ot.ReportName}",
                Value = ot.InteractionId
            }).ToList();
            options.Insert(0, new SelectListItem()
            {
                Text = "Please select a capability report",
                Value = ""
            });
            return options;
        }
    }
}
