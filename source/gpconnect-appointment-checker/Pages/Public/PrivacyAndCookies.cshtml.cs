using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class PrivacyAndCookiesModel : BaseModel
    {
        public PrivacyAndCookiesModel(IConfiguration configuration) : base(configuration)
        {
        }

        public void OnGet()
        {            
        }
    }
}