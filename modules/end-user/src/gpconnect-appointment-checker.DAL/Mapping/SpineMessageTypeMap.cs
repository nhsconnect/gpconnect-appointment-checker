using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class SpineMessageTypeMap : EntityMap<SpineMessageType>
    {
        public SpineMessageTypeMap()
        {
            Map(p => p.InteractionId).ToColumn("interaction_id");
            Map(p => p.SpineMessageTypeId).ToColumn("spine_message_type_id");
            Map(p => p.SpineMessageTypeName).ToColumn("spine_message_type_name");
        }
    }
}
