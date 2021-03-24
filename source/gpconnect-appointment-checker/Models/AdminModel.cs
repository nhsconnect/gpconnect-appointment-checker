using gpconnect_appointment_checker.DTO.Response.Application;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using gpconnect_appointment_checker.Helpers.Constants;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel
    {
        [RegularExpression(ValidationConstants.EMAILADDRESS, ErrorMessage = SearchConstants.USEREMAILADDRESSVALIDERRORMESSAGE)]
        [BindProperty]
        public string UserEmailAddress { get; set; }

        public List<User> UserList { get; set; }
        [BindProperty]
        public string SortByColumn { get; set; }

        [BindProperty]
        public string EmailAddressSearchValue { get; set; }
        [BindProperty]
        public string SurnameSearchValue { get; set; }
        [BindProperty]
        public string OrganisationNameSearchValue { get; set; }
    }
}
