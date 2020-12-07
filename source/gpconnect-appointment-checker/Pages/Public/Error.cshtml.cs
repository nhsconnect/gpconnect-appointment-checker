using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        protected ILogger<ErrorModel> _logger;

        public ErrorModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<ErrorModel> logger, ILdapService ldapService)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
