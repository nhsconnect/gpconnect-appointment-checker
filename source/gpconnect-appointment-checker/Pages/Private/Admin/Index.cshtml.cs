using System;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel : PageModel
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _contextAccessor;
        protected ILogger<AdminModel> _logger;
        protected IApplicationService _applicationService;
        protected IAuditService _auditService;
        protected readonly ILoggerManager _loggerManager;

        public AdminModel(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILogger<AdminModel> logger, IApplicationService applicationService, IAuditService auditService, ILoggerManager loggerManager = null)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _logger = logger;
            _applicationService = applicationService;
            _auditService = auditService;
            if (null != loggerManager)
            {
                _loggerManager = loggerManager;
            }
        }

        public void OnGet()
        {
            SortByColumn = SortBy.EmailAddress.ToString();
            RefreshPage();
        }

        public void OnPostSortBy(SortBy sortby)
        {
            SortByColumn = sortby.ToString();
            RefreshPage();
        }

        public void OnPostSetUserStatus(int userid, bool userstatus)
        {
            _applicationService.SetUserStatus(userid, userstatus);
            RefreshPage();
        }

        public void OnPostSetMultiSearch(int userid, bool multisearchstatus)
        {
            _applicationService.SetMultiSearch(userid, multisearchstatus);
            RefreshPage();
        }

        public IActionResult OnPostRunSearch()
        {
            var userList = _applicationService.FindUsers(SurnameSearchValue, EmailAddressSearchValue, OrganisationNameSearchValue, Enum.Parse<SortBy>(SortByColumn));
            UserList = userList;
            return Page();
        }

        public void OnPostSaveNewUser()
        {
            if (ModelState.IsValid)
            {
                UserEmailAddress = CleansedUserEmailAddress;
                _applicationService.AddUser(UserEmailAddress);
            }
            RefreshPage();
        }

        public void OnPostClearSearch()
        {
            SurnameSearchValue = null;
            EmailAddressSearchValue = null;
            OrganisationNameSearchValue = null;
            ModelState.Clear();
            RefreshPage();
        }

        private IActionResult RefreshPage()
        {
            var userList = _applicationService.GetUsers(Enum.Parse<SortBy>(SortByColumn));
            UserList = userList;
            return Page();
        }
    }
}
