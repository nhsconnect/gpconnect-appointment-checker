using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        [HttpGet("/Auth/Login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        public IActionResult Signout()
        {
            return SignOut(new AuthenticationProperties {RedirectUri = "/" }, "Cookies", "OpenIdConnect");
        }
    }
}
