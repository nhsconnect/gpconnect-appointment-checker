using gpconnect_appointment_checker.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserCreateAccount
{
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public string OrganisationName { get; set; }
    public int OrganisationId { get; set; }
    public string JobRole { get; set; }
    public string Reason { get; set; }
    public UserAccountStatus UserAccountStatus => UserAccountStatus.Pending;
}
