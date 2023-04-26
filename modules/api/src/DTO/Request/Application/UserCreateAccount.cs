using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

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
