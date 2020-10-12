using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        [HttpGet("/Auth/ExternalLogin")]
        public IActionResult ExternalLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties()
            {
                RedirectUri = returnUrl,
                Items =
                {
                    { "scheme", "nhs-sso" }
                }
            };
            return Challenge(properties, "nhs-sso");
        }

        public IActionResult Index()
        {
            return View("Public/Auth");
        }
    }
}
