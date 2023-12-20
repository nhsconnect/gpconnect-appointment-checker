using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Logging;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping
{
    public class SpineMessageMap : EntityMap<SpineMessage>
    {
        public SpineMessageMap()
        {
            Map(p => p.SpineMessageId).ToColumn("spine_message_id");
            Map(p => p.SpineMessageTypeId).ToColumn("spine_message_type_id");
            Map(p => p.UserSessionId).ToColumn("user_session_id");
            Map(p => p.Command).ToColumn("command");
            Map(p => p.RequestHeaders).ToColumn("request_headers");
            Map(p => p.RequestPayload).ToColumn("request_payload");
            Map(p => p.ResponseStatus).ToColumn("response_status");
            Map(p => p.ResponseHeaders).ToColumn("response_headers");
            Map(p => p.ResponsePayload).ToColumn("response_payload");
            Map(p => p.CreatedDate).ToColumn("logged_date");
            Map(p => p.RoundTripTimeMs).ToColumn("roundtriptime_ms");
            Map(p => p.SearchResultId).ToColumn("search_result_id");
        }
    }
}
