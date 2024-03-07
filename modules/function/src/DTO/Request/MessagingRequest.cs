namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class MessagingRequest
{
    public List<ReportSource> ReportSource { get; set; } = null;
    public List<string> Interaction { get; set; }
    public string? ReportName { get; set; } = null;
    public Guid MessageGroupId { get; set; }
}
