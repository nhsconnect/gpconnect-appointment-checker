namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class MessagingNotificationFunctionRequest
{
    public List<string> EmailAddresses { get; set; }
    public string TemplateId { get; set; }
    public string ApiKey { get; set; }
    public Dictionary<string, dynamic> TemplateParameters { get; set; } = new Dictionary<string, dynamic>();
}
