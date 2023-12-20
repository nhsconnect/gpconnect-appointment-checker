using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessDeniedModel : BaseModel
    {
        public AccessDeniedModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {            
        }

        public void OnGet()
        {            
        }
    }
}
