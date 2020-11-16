using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessDeniedModel : PageModel
    {
        public string GetAccessEmailAddress { get; set; }

        protected IConfiguration _configuration;

        public AccessDeniedModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            GetAccessEmailAddress = _configuration["General:get_access_email_address"];
        }
    }
}
