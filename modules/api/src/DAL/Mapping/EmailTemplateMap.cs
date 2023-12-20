using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class EmailTemplateMap : EntityMap<EmailTemplate>
{
    public EmailTemplateMap()
    {
        Map(p => p.EmailTemplateId).ToColumn("email_template_id");
        Map(p => p.Subject).ToColumn("subject");
        Map(p => p.Body).ToColumn("body");
    }
}

