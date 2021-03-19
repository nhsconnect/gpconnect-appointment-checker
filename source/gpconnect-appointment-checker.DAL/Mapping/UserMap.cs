using Dapper.FluentMap.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Mapping
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
            Map(p => p.OrganisationId).ToColumn("organisation_id");
            Map(p => p.UserId).ToColumn("user_id");
            Map(p => p.UserSessionId).ToColumn("user_session_id");
            Map(p => p.EmailAddress).ToColumn("email_address");
            Map(p => p.DisplayName).ToColumn("display_name");
            Map(p => p.IsAuthorised).ToColumn("is_authorised");
            Map(p => p.MultiSearchEnabled).ToColumn("multi_search_enabled");
            Map(p => p.IsAdmin).ToColumn("is_admin");
            Map(p => p.AddedDate).ToColumn("added_date");
            Map(p => p.AuthorisedDate).ToColumn("authorised_date");
            Map(p => p.LastLogonDate).ToColumn("last_logon_date");
        }
    }
}
