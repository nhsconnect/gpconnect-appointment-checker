using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using gpconnect_appointment_checker.Models;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel
    {
        [BindProperty(SupportsGet = true)] public AdminFilterModel FilterModel { get; set; }

        [Required(ErrorMessage = SearchConstants.Emailaddressrequirederrormessage)]
        [RegularExpression(ValidationConstants.Nhsnetemailaddress,
            ErrorMessage = SearchConstants.Useremailaddressvaliderrormessage)]
        [BindProperty]
        public string UserEmailAddress { get; set; }

        public string CleansedUserEmailAddress => UserEmailAddress.ToLower();

        public User[] UserList { get; set; }

        [BindProperty] public SortBy SortByColumn { get; set; }

        [BindProperty] public SortDirection SortByState { get; set; }

        [BindProperty] public string? SortByDirectionIcon { get; set; }


        [BindProperty] public int UserAccountStatusId { get; set; }

        public Dictionary<string, string> GetPaginationRouteValues(int pageNumber)
        {
            var routeValues = new Dictionary<string, string>
            {
                { "pageNumber", pageNumber.ToString() }
            };

            if (!string.IsNullOrEmpty(FilterModel?.EmailAddress))
                routeValues["FilterModel.EmailAddress"] = FilterModel.EmailAddress;

            if (!string.IsNullOrEmpty(FilterModel?.Surname))
                routeValues["FilterModel.Surname"] = FilterModel.Surname;

            // Convert Enum values to their string representation
            if (FilterModel?.AccessLevelFilter != null)
                routeValues["FilterModel.AccessLevelFilter"] = FilterModel.AccessLevelFilter.ToString();

            if (FilterModel?.UserAccountStatusFilter != null)
                routeValues["FilterModel.UserAccountStatusFilter"] = FilterModel.UserAccountStatusFilter.ToString();

            // Convert booleans to "true" or "false" strings
            if (FilterModel?.MultiSearchFilter != null)
                routeValues["FilterModel.MultiSearchFilter"] = FilterModel.MultiSearchFilter.ToString().ToLower();

            if (FilterModel?.OrgTypeSearchFilter != null)
                routeValues["FilterModel.OrgTypeSearchFilter"] = FilterModel.OrgTypeSearchFilter.ToString().ToLower();

            return routeValues;
        }
    }
}