using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
            Map(p => p.OrganisationName).ToColumn("organisation_name");
            Map(p => p.UserId).ToColumn("user_id");
            Map(p => p.UserSessionId).ToColumn("user_session_id");
            Map(p => p.EmailAddress).ToColumn("email_address");
            Map(p => p.DisplayName).ToColumn("display_name");
            Map(p => p.Status).ToColumn("status");
            Map(p => p.AccessLevel).ToColumn("access_level");
            Map(p => p.LastLogonDate).ToColumn("last_logon_date");
            Map(p => p.IsAuthorised).ToColumn("is_authorised");
            Map(p => p.MultiSearchEnabled).ToColumn("multi_search_enabled");
            Map(p => p.IsAdmin).ToColumn("is_admin");
            Map(p => p.IsNewUser).ToColumn("is_new_user");
            Map(p => p.AccessRequestCount).ToColumn("number_of_access_requests");
            Map(p => p.IsPastLastLogonThreshold).ToColumn("is_past_last_logon_threshold");
        }
    }
}
