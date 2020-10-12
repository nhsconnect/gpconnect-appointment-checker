using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public class LoginModel : PageModel
    {
        public string UserName { get; set; }
        public string OrganisationName { get; set; }
        public string OdsCode { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                UserName = User.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
                OrganisationName = User.FindFirst(c => c.Type == "Organisation")?.Value;
                OdsCode = User.FindFirst(c => c.Type == "OdsCode")?.Value;
            }
        }
    }
}
