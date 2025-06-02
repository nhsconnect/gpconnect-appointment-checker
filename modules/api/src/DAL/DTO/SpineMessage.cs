namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SpineMessage
{
    public int SpineMessageId { get; set; }

    public short SpineMessageTypeId { get; set; }

    public int? UserSessionId { get; set; }

    public string? Command { get; set; }

    public string? RequestHeaders { get; set; }

    public string RequestPayload { get; set; } = null!;

    public string? ResponseStatus { get; set; }

    public string? ResponseHeaders { get; set; }

    public string ResponsePayload { get; set; } = null!;

    public DateTime LoggedDate { get; set; }

    public double RoundtriptimeMs { get; set; }

    public int? SearchResultId { get; set; }

    public virtual SearchResult? SearchResult { get; set; }

    public virtual SpineMessageType SpineMessageType { get; set; } = null!;

    public virtual UserSession? UserSession { get; set; }
}
