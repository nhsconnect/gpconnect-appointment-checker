namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserUpdateStatus
{
    public int UserId { get; set; }
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
    public int UserAccountStatusId { get; set; }
    public string RequestUrl { get; set; }
}
