using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet("/Auth/ExternalLogin")]
        public IActionResult ExternalLogin()
        {
            var properties = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Index", "Auth"),
                Items =
                {
                    { "scheme", "nhs-sso" }
                }
            };
            return Challenge(properties, "nhs-sso");
        }

        public IActionResult Index()
        {
            return View("Auth");
        }
    }
}
