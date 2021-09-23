using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class TermsAndConditionsModel : BaseModel
    {
        public TermsAndConditionsModel(IConfiguration configuration) : base(configuration) { }

        public void OnGet()
        {            
        }
    }
}