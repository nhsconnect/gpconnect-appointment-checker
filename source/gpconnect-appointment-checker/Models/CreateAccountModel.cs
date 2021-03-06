﻿using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gpconnect_appointment_checker.Pages
{
    public partial class CreateAccountModel
    {
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }

        [Required(ErrorMessage = SearchConstants.JOBROLEREQUIREDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        public string JobRole { get; set; }

        [Required(ErrorMessage = SearchConstants.ORGANISATIONREQUIREDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        public string Organisation { get; set; }

        [Required(ErrorMessage = SearchConstants.ACCESSREQUESTREASONREQUIREDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        public string AccessRequestReason { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EmailAddress { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DisplayName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int OrganisationId { get; set; }
    }
}