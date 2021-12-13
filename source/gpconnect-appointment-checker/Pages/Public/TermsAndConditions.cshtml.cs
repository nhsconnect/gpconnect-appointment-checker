using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class TermsAndConditionsModel : BaseModel
    {
        public TermsAndConditionsModel(IOptionsMonitor<General> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor) { }

        public void OnGet()
        {            
        }
    }
}