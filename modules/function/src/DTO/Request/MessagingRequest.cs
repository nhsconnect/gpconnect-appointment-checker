namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class MessagingRequest
{
    public List<ReportSource> ReportSource { get; set; } = null;
    public string? InteractionId { get; set; } = null;
    public string? ReportName { get; set; } = null;
    public Guid MessageGroupId { get; set; }
}
