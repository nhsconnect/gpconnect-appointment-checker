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

        public IndexModel(ILogger<IndexModel> logger, IOptions<GeneralConfig> options)
        {
            _logger = logger;
            _options = options;
        }

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
