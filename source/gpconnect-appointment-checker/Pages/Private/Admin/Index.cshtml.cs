using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel : PageModel
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<AdminModel> _logger;
        protected IApplicationService _applicationService;
        protected IEmailService _emailService;
        protected IAuditService _auditService;
        protected readonly ILoggerManager _loggerManager;

        public AdminModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<AdminModel> logger, IApplicationService applicationService, IAuditService auditService, IEmailService emailService, ILoggerManager loggerManager = null)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _applicationService = applicationService;
            _emailService = emailService;
            _auditService = auditService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public void OnGet()
        {
            SortByColumn = SortBy.EmailAddress.ToString();
            SortByState = SortDirection.ASC.ToString();
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            RefreshPage();
        }

        private string GetSortDirectionIcon(string sortByState)
        {
            switch (Enum.Parse<SortDirection>(sortByState))
            {
                case SortDirection.ASC:
                    return "&nbsp;&uarr;";
                default:
                    return "&nbsp;&darr;";
            }
        }

        public void OnPostSortBy(SortBy sortby, SortDirection sortDirection)
        {
            ClearValidationState();
            SortByColumn = sortby.ToString();
            SortByState = sortDirection.ToString();
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            RefreshPage();
        }

        public void OnPostSetUserStatuses(int[] UserId, int[] UserAccountStatusId)
        {
            ClearValidationState();
            var users = _applicationService.SetUserStatus(UserId, UserAccountStatusId);
            foreach (var user in users)
            {
                _emailService.SendUserStatusEmail(user.UserId, user.UserAccountStatusId, user.EmailAddress);
            }
            RefreshPage();
        }

        public void OnPostSetMultiSearch(int userid, bool multisearchstatus)
        {
            _logger.LogInformation("Called OnPostSetMultiSearch");
            ClearValidationState();
            _logger.LogInformation("Called _applicationService.SetMultiSearch");
            _applicationService.SetMultiSearch(userid, multisearchstatus);
            RefreshPage();
        }

        private void ClearValidationState()
        {
            ModelState.ClearValidationState("UserEmailAddress");
        }

        public IActionResult OnPostRunSearch()
        {
            ClearValidationState();
            var userList = _applicationService.FindUsers(SurnameSearchValue, EmailAddressSearchValue, OrganisationNameSearchValue, Enum.Parse<SortBy>(SortByColumn));
            UserList = userList;
            return Page();
        }

        public IActionResult OnPostFilterByStatus()
        {
            ClearValidationState();
            var userList = _applicationService.GetUsers(Enum.Parse<SortBy>(SortByColumn), Enum.Parse<SortDirection>(SortByState), string.IsNullOrEmpty(SelectedUserAccountStatusFilter) ? null : Enum.Parse<UserAccountStatus>(SelectedUserAccountStatusFilter));
            UserList = userList;
            return Page();
        }

        public void OnPostSaveNewUser()
        {
            if (ModelState.IsValid)
            {
                UserEmailAddress = CleansedUserEmailAddress;
                var user = _applicationService.AddUser(UserEmailAddress);
                if (user != null && user.IsNewUser)
                {
                    _emailService.SendUserStatusEmail(user.UserId, user.UserAccountStatusId, user.EmailAddress);
                }
                UserEmailAddress = null;
            }
            ModelState.Clear();
            RefreshPage();
        }

        public void OnPostClearSearch()
        {
            ClearValidationState();
            SurnameSearchValue = null;
            EmailAddressSearchValue = null;
            OrganisationNameSearchValue = null;
            ModelState.Clear();
            RefreshPage();
        }

        private IActionResult RefreshPage()
        {
            var userList = _applicationService.GetUsers(Enum.Parse<SortBy>(SortByColumn), Enum.Parse<SortDirection>(SortByState));
            UserList = userList;
            return Page();
        }
    }
}