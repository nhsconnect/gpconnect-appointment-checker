namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class UserRequestParameters
{
    public string FamilyName { get; set; }
    public string GivenName { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public string Sid { get; set; }
    public int UserId { get; set; }
}
