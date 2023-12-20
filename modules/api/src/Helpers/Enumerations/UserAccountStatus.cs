using System.ComponentModel;

namespace GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

public enum UserAccountStatus : ushort
{
    [Description("Unknown")]
    Unknown = 0,
    [Description("Pending")] 
    Pending = 1,
    [Description("Authorised")]
    Authorised = 2,
    [Description("Deauthorised")]
    Deauthorised = 3,
    [Description("Request Denied")]
    RequestDenied = 4
}
