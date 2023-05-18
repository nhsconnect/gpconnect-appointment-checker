namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserUpdateMultiSearch
{
    public int UserId { get; set; }
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
    public bool MultiSearchEnabled { get; set; }
}