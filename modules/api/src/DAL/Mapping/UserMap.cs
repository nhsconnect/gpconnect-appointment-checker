using Dapper.FluentMap.Mapping;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.DAL.Mapping;

public class UserMap : EntityMap<User>
{
    public UserMap()
    {
        Map(p => p.UserId).ToColumn("user_id");
        Map(p => p.OrganisationName).ToColumn("organisation_name");
        Map(p => p.OrganisationId).ToColumn("organisation_id");
        Map(p => p.UserSessionId).ToColumn("user_session_id");
        Map(p => p.EmailAddress).ToColumn("email_address");
        Map(p => p.DisplayName).ToColumn("display_name");
        Map(p => p.UserAccountStatusId).ToColumn("user_account_status_id");
        Map(p => p.AccessLevel).ToColumn("access_level");
        Map(p => p.LastLogonDate).ToColumn("last_logon_date");
        Map(p => p.MultiSearchEnabled).ToColumn("multi_search_enabled");
        Map(p => p.IsAdmin).ToColumn("is_admin");
        Map(p => p.IsNewUser).ToColumn("is_new_user");
        Map(p => p.AccessRequestCount).ToColumn("number_of_access_requests");
        Map(p => p.IsPastLastLogonThreshold).ToColumn("is_past_last_logon_threshold");
        Map(p => p.StatusChanged).ToColumn("status_changed");
        Map(p => p.OrgTypeSearchEnabled).ToColumn("org_type_search_enabled");
    }
}
