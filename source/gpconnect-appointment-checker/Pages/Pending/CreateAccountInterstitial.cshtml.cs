using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class CreateAccountInterstitialModel : BaseModel
    {
        public CreateAccountInterstitialModel(IConfiguration configuration) : base(configuration)
        {            
        }

        public void OnGet()
        {            
        }
    }
}
