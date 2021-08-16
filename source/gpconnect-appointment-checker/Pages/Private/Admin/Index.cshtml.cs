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

        public void OnPostSetUserAccountStatus(int accountstatususerid, int userselectedindex, int[] UserAccountStatusId)
        {
            ClearValidationState();
            var user = _applicationService.SetUserStatus(accountstatususerid, UserAccountStatusId[userselectedindex]);
            if (user.StatusChanged)
            {
                _emailService.SendUserStatusEmail(user.UserId, user.UserAccountStatusId, user.EmailAddress);
            }
            RefreshPage();
        }

        public void OnPostSetMultiSearch(int multisearchstatususerid, bool multisearchstatus)
        {
            ClearValidationState();
            _applicationService.SetMultiSearch(multisearchstatususerid, multisearchstatus);
            RefreshPage();
        }

        public void OnPostSetOrgTypeSearch(int orgtypesearchstatususerid, bool orgtypesearchstatus)
        {
            ClearValidationState();
            _applicationService.SetOrgTypeSearch(orgtypesearchstatususerid, orgtypesearchstatus);
            RefreshPage();
        }

        private void ClearValidationState()
        {
            ModelState.ClearValidationState("UserEmailAddress");
        }

        public IActionResult OnPostApplyFilter()
        {
            ClearValidationState();
            var userList = _applicationService.GetUsers(
                SurnameSearchValue, 
                EmailAddressSearchValue, 
                OrganisationNameSearchValue, 
                Enum.Parse<SortBy>(SortByColumn), 
                Enum.Parse<SortDirection>(SortByState), 
                string.IsNullOrEmpty(SelectedUserAccountStatusFilter) ? null : Enum.Parse<UserAccountStatus>(SelectedUserAccountStatusFilter),
                string.IsNullOrEmpty(SelectedAccessLevelFilter) ? null : Enum.Parse<AccessLevel>(SelectedAccessLevelFilter),
                string.IsNullOrEmpty(SelectedMultiSearchFilter) ? null : bool.Parse(SelectedMultiSearchFilter),
                string.IsNullOrEmpty(SelectedOrgTypeSearchFilter) ? null : bool.Parse(SelectedOrgTypeSearchFilter)
                );
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
            RefreshPage();
        }

        public void OnPostClearSearch()
        {
            ClearValidationState();
            SurnameSearchValue = null;
            EmailAddressSearchValue = null;
            OrganisationNameSearchValue = null;
            SelectedAccessLevelFilter = null;
            SelectedMultiSearchFilter = null;
            SelectedOrgTypeSearchFilter = null;
            SelectedUserAccountStatusFilter = null;
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