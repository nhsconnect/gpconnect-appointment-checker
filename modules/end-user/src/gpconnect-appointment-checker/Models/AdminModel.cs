using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel
    {
        [Required(ErrorMessage = SearchConstants.EMAILADDRESSREQUIREDERRORMESSAGE)]
        [RegularExpression(ValidationConstants.NHSNETEMAILADDRESS, ErrorMessage = SearchConstants.USEREMAILADDRESSVALIDERRORMESSAGE)]
        [BindProperty]
        public string UserEmailAddress { get; set; }

        public string CleansedUserEmailAddress => UserEmailAddress.ToLower();

        public List<User> UserList { get; set; }

        [BindProperty]
        public SortBy SortByColumn { get; set; }

        [BindProperty]
        public SortDirection SortByState { get; set; }

        [BindProperty]
        public string? SortByDirectionIcon { get; set; }
        [BindProperty]
        public UserAccountStatus? SelectedUserAccountStatusFilter { get; set; }
        [BindProperty]
        public AccessLevel? SelectedAccessLevelFilter { get; set; }

        [BindProperty]
        public bool? SelectedOrgTypeSearchFilter { get; set; }

        [BindProperty]
        public bool? SelectedMultiSearchFilter { get; set; }

        [BindProperty]
        public int UserAccountStatusId { get; set; }

        [BindProperty]
        public string? EmailAddressSearchValue { get; set; }
        [BindProperty]
        public string? SurnameSearchValue { get; set; }
        [BindProperty]
        public string? OrganisationNameSearchValue { get; set; }
    }
}
