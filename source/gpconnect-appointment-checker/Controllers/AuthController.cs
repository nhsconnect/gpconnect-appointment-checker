using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Application;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IApplicationService applicationService, IHttpContextAccessor contextAccessor, ILogger<AuthController> logger)
        {
            _applicationService = applicationService;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        [HttpGet("/Auth/Login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }

        public IActionResult Signout()
        {
            var signOutResult = new SignOutResult();
            var userSessionId = _contextAccessor.HttpContext.User.GetClaimValue("UserSessionId").StringToInteger(0);
            try
            {
                if (userSessionId > 0)
                {
                    _applicationService.LogoffUser(new User
                    {
                        EmailAddress = _contextAccessor.HttpContext.User.GetClaimValue("Email"),
                        UserSessionId = userSessionId
                    });
                }
            }
            catch (Exception exc)
            {
                _logger.LogError("An error occurred in trying to logout the user", exc);
                throw;
            }
            finally
            {
                signOutResult = SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "OpenIdConnect");
            }
            return signOutResult;
        }
    }
}
