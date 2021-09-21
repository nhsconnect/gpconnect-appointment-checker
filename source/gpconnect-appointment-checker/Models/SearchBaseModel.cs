using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchBaseModel : BaseModel
    {
        private readonly IReportingService _reportingService;
        private readonly IHttpContextAccessor _contextAccessor;
        protected readonly int _userId;
        protected readonly bool _multiSearchEnabled;
        protected readonly bool _orgTypeSearchEnabled;

        public SearchBaseModel(IHttpContextAccessor contextAccessor, IReportingService reportingService)
        {
            _reportingService = reportingService;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext != null)
            {
                _userId = _contextAccessor.HttpContext.User.GetClaimValue("UserId").StringToInteger();
                _multiSearchEnabled = _contextAccessor.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false);
                _orgTypeSearchEnabled = _contextAccessor.HttpContext.User.GetClaimValue("OrgTypeSearchEnabled").StringToBoolean(false);
            }

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
            var memoryStream = _reportingService.CreateReport(dataTable, ReportConstants.SLOTSEARCHREPORTHEADING);
            return GetFileStream(memoryStream);
        }
    }
}