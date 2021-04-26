﻿using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void OnPostSetUserStatuses()
        {
            ClearValidationState();

            var selectedUserAccountStatus = SelectedUserAccountStatus;
            //_applicationService.SetUserStatus(userid, selectedUserAccountStatus);
            RefreshPage();
        }

        public void OnPostSetMultiSearch(int userid, bool multisearchstatus)
        {
            ClearValidationState();
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
            var userList = _applicationService.GetUsers(Enum.Parse<SortBy>(SortByColumn), Enum.Parse<SortDirection>(SortByState), Enum.Parse<UserAccountStatus>(SelectedUserAccountStatusFilter));
            UserList = userList;
            return Page();
        }

        public void OnPostSaveNewUser()
        {
            if (ModelState.IsValid)
            {
                UserEmailAddress = CleansedUserEmailAddress;
                _applicationService.AddUser(UserEmailAddress);
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
            //SelectedStatusFilter = StatusFilter.All.ToString();
            ModelState.Clear();
            RefreshPage();
        }

        private IActionResult RefreshPage()
        {
            var userList = _applicationService.GetUsers(Enum.Parse<SortBy>(SortByColumn), Enum.Parse<SortDirection>(SortByState));
            UserList = userList;
            return Page();
        }

        public IEnumerable<SelectListItem> GetUserAccountStatuses(UserAccountStatus? userAccountStatus, bool includeDefault = false)
        {
            List<SelectListItem> valuesList = new List<SelectListItem>();
            if (includeDefault)
            {
                valuesList.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            }

            IEnumerable<SelectListItem> values = Enum.GetValues(typeof(UserAccountStatus)).Cast<UserAccountStatus>().Select(v => new SelectListItem
            {
                Text = v.GetDescription(),
                Value = v.ToString(),
                Selected = userAccountStatus != null ? userAccountStatus == v : false
            });
    
            valuesList.AddRange(values);            
            
            return valuesList;
        }
    }
}
