using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class EmailTemplateMap : EntityMap<EmailTemplate>
    {
        public EmailTemplateMap()
        {
            Map(p => p.EmailTemplateId).ToColumn("email_template_id");
            Map(p => p.Subject).ToColumn("subject");
            Map(p => p.Body).ToColumn("body");
        }
    }
}
