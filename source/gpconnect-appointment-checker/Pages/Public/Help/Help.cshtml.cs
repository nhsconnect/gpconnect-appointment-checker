using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class HelpModel : BaseModel
    {
        public HelpModel(IConfiguration configuration) : base(configuration)
        {            
        }

        public void OnGet()
        {            
        }
    }
}
