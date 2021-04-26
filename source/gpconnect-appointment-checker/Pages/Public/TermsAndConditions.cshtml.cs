using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public class TermsAndConditionsModel : PageModel
    {
        public string GetAccessEmailAddress { get; set; }
        public bool AcceptsTerms { get; set; }

        protected ILogger<TermsAndConditionsModel> _logger;
        protected IConfiguration _configuration;
        protected IApplicationService _applicationService;
        protected readonly ILoggerManager _loggerManager;

        public TermsAndConditionsModel(IConfiguration configuration, ILogger<TermsAndConditionsModel> logger, IApplicationService applicationService, ILoggerManager loggerManager = null)
        {
            _configuration = configuration;
            _applicationService = applicationService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public void OnGet()
        {
            GetAccessEmailAddress = _configuration["General:get_access_email_address"];
        }

        public async Task<IActionResult> OnPostAcceptTermsAsync()
        {
            _applicationService.UpdateUserTermsAndConditions(AcceptsTerms);
            return Page();
        }
    }
}