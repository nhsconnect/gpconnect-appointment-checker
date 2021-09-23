using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessibilityModel : BaseModel
    {
        public AccessibilityModel(IConfiguration configuration) : base(configuration) { }

        public void OnGet()
        {
        }
    }
}
