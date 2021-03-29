﻿using gpconnect_appointment_checker.DTO.Response.Application;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel
    {
        [Required(ErrorMessage = SearchConstants.EMAILADDRESSREQUIREDERRORMESSAGE)]
        [RegularExpression(ValidationConstants.NHSNETEMAILADDRESS, ErrorMessage = SearchConstants.USEREMAILADDRESSVALIDERRORMESSAGE)]
        [BindProperty]
        public string UserEmailAddress { get; set; }

        public string CleansedUserEmailAddress => UserEmailAddress.ToLower();

        public List<SelectListItem> StatusFilters => GetStatusFilters();

        public List<User> UserList { get; set; }
        [BindProperty]
        public string SortByColumn { get; set; }

        [BindProperty]
        public string SortByState { get; set; }

        [BindProperty]
        public string SortByDirectionIcon { get; set; }
        [BindProperty]
        public string SelectedStatusFilter { get; set; }

        [BindProperty]
        public string EmailAddressSearchValue { get; set; }
        [BindProperty]
        public string SurnameSearchValue { get; set; }
        [BindProperty]
        public string OrganisationNameSearchValue { get; set; }
    }
}
