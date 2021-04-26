using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace gpconnect_appointment_checker.Pages
{
    public partial class CreateAccountModel
    {
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
    }
}