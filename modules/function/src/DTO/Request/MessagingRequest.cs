namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class MessagingRequest
{
    public List<ReportSource> ReportSource { get; set; }
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string? ReportName { get; set; } = null;
    public Guid MessageGroupId { get; set; }
}
