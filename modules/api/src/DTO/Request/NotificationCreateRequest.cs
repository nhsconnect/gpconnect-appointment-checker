using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class NotificationCreateRequest
{
    public string? ApiKey { get; set; }
    public List<string> EmailAddresses { get; set; }
    public string TemplateId { get; set; }
    public string? RequestUrl { get; set; }
    public Dictionary<string, dynamic> TemplateParameters { get; set; } = new Dictionary<string, dynamic>();
    public Dictionary<string, byte[]>? FileUpload { get; set; } = new Dictionary<string, byte[]>();
}
