using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class SpineMessageTypeMap : EntityMap<SpineMessageType>
{
    public SpineMessageTypeMap()
    {
        Map(p => p.SpineMessageTypeId).ToColumn("spine_message_type_id");
        Map(p => p.SpineMessageTypeName).ToColumn("spine_message_type_name");
        Map(p => p.InteractionId).ToColumn("interaction_id");
    }
}
