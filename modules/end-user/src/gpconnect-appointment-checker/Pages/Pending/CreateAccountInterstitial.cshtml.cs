using GpConnect.AppointmentChecker.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class CreateAccountInterstitialModel : BaseModel
    {
        public CreateAccountInterstitialModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {            
        }

        public void OnGet()
        {            
        }
    }
}
