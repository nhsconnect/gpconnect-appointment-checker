using GpConnect.AppointmentChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class TermsAndConditionsModel : BaseModel
    {
        public TermsAndConditionsModel(IOptions<General> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor) { }

        public void OnGet()
        {            
        }
    }
}