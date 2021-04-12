using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class EmailTemplate
    {
        public int EmailTemplateId { get; set; }

        public MailTemplate MailTemplate => (MailTemplate) EmailTemplateId;

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
