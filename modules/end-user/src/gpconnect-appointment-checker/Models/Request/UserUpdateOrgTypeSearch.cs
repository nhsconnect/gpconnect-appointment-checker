using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserUpdateOrgTypeSearch
{
    public int UserId { get; set; }
    public bool OrgTypeSearchEnabled { get; set; }
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
    public Uri RequestUrl { get; set; }
}
