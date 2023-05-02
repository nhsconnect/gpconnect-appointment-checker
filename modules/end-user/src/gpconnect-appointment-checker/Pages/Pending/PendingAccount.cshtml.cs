using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class PendingAccountModel : BaseModel
    {
        public PendingAccountModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor) { }

        public void OnGet()
        {
        }
    }
}