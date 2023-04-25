using GpConnect.AppointmentChecker.Core.Config;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public partial class CreateAccountModel : BaseModel
    {
        protected ILogger<CreateAccountModel> _logger;
        protected IHttpContextAccessor _contextAccessor;
        protected readonly ILoggerManager _loggerManager;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IOptions<NotificationConfig> _notificationConfig;
        private readonly IOptions<General> _configuration;

        public CreateAccountModel(INotificationService notificationService, IOptions<NotificationConfig> notificationConfig, IOptions<General> configuration, IHttpContextAccessor contextAccessor, ILogger<CreateAccountModel> logger, IUserService userService, ILoggerManager loggerManager = null) : base(configuration, contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
            _notificationService = notificationService;
            _configuration = configuration;
            _notificationConfig = notificationConfig;
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

        public async Task<IActionResult> OnPostSendForm()
        {
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
                var createdUser = await _userService.AddOrUpdateUser(userCreateAccount);

                await _notificationService.PostNotificationAsync(new NotificationDetails
                {
                    EmailAddresses = new List<string>() { _configuration.Value.GetAccessEmailAddress },
                    TemplateId = _notificationConfig.Value.UserDetailsFormTemplateId,
                    TemplateParameters = new Dictionary<string, dynamic> {
                    { "email_address", userCreateAccount.EmailAddress },
                    { "job_role", userCreateAccount.JobRole },
                    { "organisation_name", userCreateAccount.OrganisationName },
                    { "access_reason", userCreateAccount.Reason }
                }
                });

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
