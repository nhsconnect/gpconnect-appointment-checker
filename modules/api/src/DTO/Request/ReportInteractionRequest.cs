namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportInteractionRequest
{
    public List<ReportSource> ReportSource { get; set; }
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string? ReportName { get; set; } = null;
    public Guid? MessageGroupId { get; set; } = Guid.NewGuid();
}