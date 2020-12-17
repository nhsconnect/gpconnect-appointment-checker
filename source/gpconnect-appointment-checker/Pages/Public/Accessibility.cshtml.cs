using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessibilityModel : PageModel
    {
        public string GetAccessEmailAddress { get; set; }
        public string LastUpdated { get; set; }

        protected IConfiguration _configuration;

        public AccessibilityModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            GetAccessEmailAddress = _configuration["General:get_access_email_address"];
            LastUpdated = $"{DateTime.UtcNow:MMMM yyyy}";
        }
    }
}
