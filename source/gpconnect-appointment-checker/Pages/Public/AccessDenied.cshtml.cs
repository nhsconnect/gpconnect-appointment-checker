using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessDeniedModel : BaseModel
    {
        public AccessDeniedModel(IConfiguration configuration) : base(configuration)
        {            
        }

        public void OnGet()
        {            
        }
    }
}
