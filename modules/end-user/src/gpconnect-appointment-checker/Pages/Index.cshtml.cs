using GpConnect.AppointmentChecker.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace gpconnect_appointment_checker.Pages
{
    [Authorize(Policy = "CanBeAuthorisedOrNotAuthorisedUserStatus")]
    public class IndexModel : PageModel
    {
        protected ILogger<IndexModel> _logger;
        private readonly IOptions<GeneralConfig> _options;
        private readonly IOptions<SingleSignOnConfig> _ssoOptions;

        public IndexModel(ILogger<IndexModel> logger, IOptions<GeneralConfig> options, IOptions<SingleSignOnConfig> ssoOptions)
        {
            _logger = logger;
            _options = options;
            _ssoOptions = ssoOptions;
        }

        public IActionResult OnGet()
        {
            SsoClientId = _ssoOptions.Value.ClientId;
            return Page();
        }

        public string SsoClientId { get; set; }
    }
}
