namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class NotificationCreateRequest
{
    public List<string> EmailAddresses { get; set; }
    public string TemplateId { get; set; }
    public Dictionary<string, dynamic> TemplateParameters { get; set; } = new Dictionary<string, dynamic>();
}
