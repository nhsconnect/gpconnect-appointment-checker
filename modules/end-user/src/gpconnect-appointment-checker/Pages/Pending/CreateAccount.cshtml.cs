using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using gpconnect_appointment_checker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Pages
{
    public partial class CreateAccountModel : BaseModel
    {
        protected ILogger<CreateAccountModel> _logger;
        protected IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public CreateAccountModel(IOptions<GeneralConfig> configuration,
            IHttpContextAccessor contextAccessor,
            ILogger<CreateAccountModel> logger,
            IUserService userService) : base(configuration, contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        public IActionResult OnGet()
        {
            UserName = User.GetClaimValue("DisplayName");
            Organisation = User.GetClaimValue("OrganisationName");
            OrganisationId = User.GetClaimValue("OrganisationId").StringToInteger();
            EmailAddress = User.GetClaimValue("Email");
            DisplayName = User.GetClaimValue("DisplayName");

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
                    OrganisationId = OrganisationId,
                    RequestUrl = FullUrl
                };
                await _userService.AddOrUpdateUser(userCreateAccount);
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
