namespace GpConnect.AppointmentChecker.Api.DTO.Request.Logging;

public class ErrorLog
{
    public int UserId { get; set; } = 0;
    public int UserSessionId { get; set; } = 0;

    public string Application { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string Logger { get; set; }
    public string Callsite { get; set; }
    public string Exception { get; set; }
}
