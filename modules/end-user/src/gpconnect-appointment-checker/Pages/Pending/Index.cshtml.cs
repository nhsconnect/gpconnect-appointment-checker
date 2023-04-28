using GpConnect.AppointmentChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class PendingIndexModel : BaseModel
    {
        public PendingIndexModel(IOptions<General> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {
        }

        public void OnGet()
        {
        }
    }
}
