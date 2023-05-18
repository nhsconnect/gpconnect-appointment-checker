namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserUpdateOrgTypeSearch
{
    public int UserId { get; set; }
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
    public bool OrgTypeSearchEnabled { get; set; }
}

