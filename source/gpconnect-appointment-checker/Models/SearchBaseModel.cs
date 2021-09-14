using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchBaseModel : PageModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IReportingService _reportingService;

        public SearchBaseModel(IApplicationService applicationService, IReportingService reportingService)
        {
            _applicationService = applicationService;
            _reportingService = reportingService;
        }

        public List<List<SlotEntrySimple>> SearchResults { get; set; }
        public List<List<SlotEntrySimple>> SearchResultsPast { get; set; }

        [BindProperty]
        public string SearchAtResultsText { get; set; }
        [BindProperty]
        public string SearchOnBehalfOfResultsText { get; set; }
        
        [BindProperty(Name = "SearchGroupId", SupportsGet = true)]
        public int SearchGroupId { get; set; }

        [BindProperty(Name = "SearchResultId", SupportsGet = true)]
        public int SearchResultId { get; set; }

        [BindProperty(Name = "SearchExportId", SupportsGet = true)]
        public int SearchExportId { get; set; }
        public double SearchDuration { get; set; }
        
        public int SearchResultsTotalCount { get; set; }
        public int SearchResultsCurrentCount { get; set; }
        public int SearchResultsPastCount { get; set; }

        public string ProviderPublisher { get; set; }

        protected FileStreamResult ExportSearchResults(int searchexportid)
        {
            var userId = User.GetClaimValue("UserId").StringToInteger();
            var dataTable = _applicationService.GetSearchExport(searchexportid, userId);
            var memoryStream = _reportingService.CreateReport(dataTable, ReportConstants.SLOTSEARCHREPORTHEADING);
            return GetFileStream(memoryStream);
        }

        protected static FileStreamResult GetFileStream(MemoryStream memoryStream, string fileName = null)
        {
            return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = fileName ?? $"{DateTime.UtcNow.ToFileTimeUtc()}.xlsx"
            };
        }
    }
}