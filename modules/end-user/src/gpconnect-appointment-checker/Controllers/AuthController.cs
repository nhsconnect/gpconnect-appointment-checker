using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Application;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IHttpContextAccessor contextAccessor, ILogger<AuthController> logger)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        [Route("/Auth/Login")]
        [AllowAnonymous]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(30)
            });
        }

        [Route("/Auth/Register")]
        [AllowAnonymous]
        public async Task Register()
        {
            await HttpContext.ChallengeAsync("OpenIdConnect", new AuthenticationProperties
            {
                RedirectUri = "/CreateAccount",
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(30)
            });
        }

        [Route("/Auth/Logout")]
        public IActionResult Logout()
        {
            var userSessionId = _contextAccessor.HttpContext.User.GetClaimValue("UserSessionId").StringToInteger(0);
            try
            {
                if (userSessionId > 0)
                {
                    _userService.LogoffUser(new LogoffUser
                    {
                        EmailAddress = _contextAccessor.HttpContext.User.GetClaimValue("Email"),
                        UserSessionId = userSessionId
                    });
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to logout the user");
                throw;
            }
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
