using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Application;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Pages
{
    public partial class CreateAccountModel : BaseModel
    {
        protected ILogger<CreateAccountModel> _logger;
        protected IHttpContextAccessor _contextAccessor;
        protected IApplicationService _applicationService;
        protected IEmailService _emailService;
        protected readonly ILoggerManager _loggerManager;

        public CreateAccountModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<CreateAccountModel> logger, IApplicationService applicationService, IEmailService emailService, ILoggerManager loggerManager = null) : base(configuration)
        {
            _contextAccessor = contextAccessor;
            _applicationService = applicationService;
            _emailService = emailService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public IActionResult OnGet()
        {
            UserName = User.GetClaimValue("DisplayName");
            Organisation = User.GetClaimValue("OrganisationName");
            OrganisationId = User.GetClaimValue("OrganisationId").StringToInteger();
            EmailAddress = User.GetClaimValue("Email");

            ModelState.ClearValidationState("JobRole");
            ModelState.ClearValidationState("Organisation");
            ModelState.ClearValidationState("AccessRequestReason");
            return Page();
        }

        public IActionResult OnPostSendForm()
        {
            var emailSent = false;
            if (ModelState.IsValid)
            {
                var userCreateAccount = new UserCreateAccount
                {
                    EmailAddress = EmailAddress,
                    JobRole = JobRole,
                    OrganisationName = Organisation,
                    Reason = AccessRequestReason,
                    DisplayName = UserName,
                    OrganisationId = OrganisationId
                };
                var createdUser = _applicationService.AddOrUpdateUser(userCreateAccount);               

                if (createdUser != null && createdUser.UserAccountStatusId == (int)UserAccountStatus.Pending)
                {
                    emailSent = _emailService.SendUserCreateAccountEmail(createdUser, userCreateAccount);                    
                }

                TempData["EmailAddressManual"] = GetAccessEmailAddress;
                TempData["EmailSent"] = emailSent;

                return Redirect("/Pending/Index");
            }
            return Page();
        }

        public IActionResult OnPostClear()
        {
            JobRole = null;
            Organisation = User.GetClaimValue("OrganisationName");
            AccessRequestReason = null;
            ModelState.Clear();
            return Page();
        }
    }
}
