namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class WebRequest
{
    public int WebRequestId { get; set; }

    public int? UserId { get; set; }

    public int? UserSessionId { get; set; }

    public string Url { get; set; } = null!;

    public string? ReferrerUrl { get; set; }

    public string Description { get; set; } = null!;

    public string Ip { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string Server { get; set; } = null!;

    public int ResponseCode { get; set; }

    public string SessionId { get; set; } = null!;

    public string UserAgent { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual UserSession? UserSession { get; set; }
}
