using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace gpconnect_appointment_checker.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class TokenExpirationModel : PageModel
    {
        protected ILogger<TokenExpirationModel> _logger;

        public TokenExpirationModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<TokenExpirationModel> logger, ILdapService ldapService)
        {
            _logger = logger;
        }

        public string RequestId { get; set; }
        public string ExceptionMessage { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError($"Timeout error thrown at {exceptionHandlerPathFeature?.Path} - ", exceptionHandlerPathFeature?.Error.InnerException);
        }
    }
}
