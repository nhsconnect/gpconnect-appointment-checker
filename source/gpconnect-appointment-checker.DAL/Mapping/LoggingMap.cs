using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Logging;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class LoggingMap : EntityMap<PurgeErrorLog>
    {
        public LoggingMap()
        {
            Map(p => p.ErrorLogDeletedCount).ToColumn("error_log_deleted_count");
            Map(p => p.SpineMessageDeletedCount).ToColumn("spine_message_deleted_count");
            Map(p => p.WebRequestDeletedCount).ToColumn("web_request_deleted_count");
        }
    }
}
