using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class PrivacyAndCookiesModel : PageModel
    {
        public string GetAccessEmailAddress { get; set; }
        public string ApplicationName { get; set; }
        public string LastUpdated { get; set; }
        protected IConfiguration _configuration;

        public PrivacyAndCookiesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            GetAccessEmailAddress = _configuration["General:get_access_email_address"];
            ApplicationName = _configuration["General:product_name"];
            LastUpdated = $"{DateTime.UtcNow:MMMM yyyy}";
        }
    }
}
