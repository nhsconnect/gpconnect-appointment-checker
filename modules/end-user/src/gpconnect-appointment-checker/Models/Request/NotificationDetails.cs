using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models.Request;

public class NotificationDetails
{
    public List<string> EmailAddresses { get; set; }
    public string TemplateId { get; set; }
    public Dictionary<string, dynamic> TemplateParameters { get; set; } = new Dictionary<string, dynamic>();
}
