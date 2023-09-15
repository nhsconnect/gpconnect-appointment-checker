using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace gpconnect_appointment_checker.Helpers.Enumerations
{
    public enum UserAccountStatus : ushort
    {
        [Description("Unknown")]
        [Display(Name = "Unknown")]
        Unknown = 0,
        [Description("Pending")]
        [Display(Name = "Pending")]
        Pending = 1,
        [Description("Authorised")]
        [Display(Name = "Authorised")]
        Authorised = 2,
        [Description("Deauthorised")]
        [Display(Name = "Deauthorised")]
        Deauthorised = 3,
        [Description("Request Denied")]
        [Display(Name = "Request Denied")]
        RequestDenied = 4
    }
}
