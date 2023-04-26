namespace GpConnect.AppointmentChecker.Api.DTO.Request.Logging;

public class WebRequest
{
    public int UserId { get; set; } = 0;
    public int UserSessionId { get; set; } = 0;
    public string Url { get; set; }
    public string ReferrerUrl { get; set; }
    public string Description { get; set; }
    public string Ip { get; set; }
    public string CreatedBy { get; set; }
    public string Server { get; set; }
    public int ResponseCode { get; set; }
    public string SessionId { get; set; }
    public string UserAgent { get; set; }
}
