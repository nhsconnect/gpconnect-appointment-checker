using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    public class AccessibilityModel : BaseModel
    {
        public AccessibilityModel(IOptions<General> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor) { }

        public void OnGet()
        {
        }
    }
}
