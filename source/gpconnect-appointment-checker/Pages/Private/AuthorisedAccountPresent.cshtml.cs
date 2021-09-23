using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class AuthorisedAccountPresentModel : BaseModel
    {
        public AuthorisedAccountPresentModel(IConfiguration configuration) : base(configuration)
        {
        }

        public void OnGet()
        {            
        }
    }
}
