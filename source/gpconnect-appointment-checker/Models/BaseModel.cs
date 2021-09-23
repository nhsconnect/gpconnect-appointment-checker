using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace gpconnect_appointment_checker.Pages
{
    public class BaseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string GetAccessEmailAddress { get; set; }
        public string GetAccessEmailAddressTag => $"<a href=\"mailto:{GetAccessEmailAddress}\">{GetAccessEmailAddress}</a>";
        public int MaxNumberConsumerCodesSearch { get; }
        public int MaxNumberProviderCodesSearch { get; }
        public int MaxNumberWeeksSearch { get; }
        public string LastUpdated => $"{DateTime.UtcNow:MMMM yyyy}";
        public string ApplicationName { get; set; }

        public BaseModel(IConfiguration configuration)
        {
            _configuration = configuration;
            if(_configuration != null)
            {
                GetAccessEmailAddress = _configuration["General:get_access_email_address"];
                MaxNumberProviderCodesSearch = _configuration["General:max_number_provider_codes_search"].StringToInteger(20);
                MaxNumberConsumerCodesSearch = _configuration["General:max_number_consumer_codes_search"].StringToInteger(20);
                MaxNumberWeeksSearch = _configuration["General:max_num_weeks_search"].StringToInteger(12);
                ApplicationName = _configuration["General:product_name"];
            }
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