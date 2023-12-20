namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class NotificationConfig
{
    public string ApiKey { get; set; }
    public string AccountDeactivatedTemplateId { get; set; }
    public string NewAccountCreatedTemplateId { get; set; }
    public string UserDetailsFormTemplateId { get; set; }
}