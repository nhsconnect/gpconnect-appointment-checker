using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Application;

public class EmailTemplate
{
    public int EmailTemplateId { get; set; }

    public MailTemplate MailTemplate => (MailTemplate) EmailTemplateId;

    public string Subject { get; set; }

    public string Body { get; set; }
}
