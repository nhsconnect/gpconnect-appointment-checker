namespace GpConnect.AppointmentChecker.Models.Request;

public class LogonUser
{
    public int OrganisationId { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
}
