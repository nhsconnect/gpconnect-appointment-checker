using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchBaseModel : BaseModel
    {
        private readonly IReportingService _reportingService;

        public SearchBaseModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor, IReportingService reportingService) : base(configuration, contextAccessor)
        {
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

        protected FileStreamResult ExportResult(DataTable dataTable)
        {
            //var memoryStream = _reportingService.CreateReport(dataTable, ReportConstants.SLOTSEARCHREPORTHEADING);
            //return GetFileStream(memoryStream);
            return null;
        }
    }
}