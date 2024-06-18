namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportInteractionRequest
{
    public DataSource DataSource { get; set; }
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string? ReportName { get; set; } = null;
    public string? ReportId { get; set; } = null;
    public Guid MessageGroupId { get; set; }
}