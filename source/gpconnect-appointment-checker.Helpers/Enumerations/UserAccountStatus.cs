using System.ComponentModel;

namespace gpconnect_appointment_checker.Helpers.Enumerations
{
    public enum UserAccountStatus : ushort
    {
        [Description("Pending")] 
        Pending = 1,
        [Description("Authorised")]
        Authorised = 2,
        [Description("Deauthorised")]
        Deauthorised = 3,
        [Description("Request Denied")]
        RequestDenied = 4
    }
}
