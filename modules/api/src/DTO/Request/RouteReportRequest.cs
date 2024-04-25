namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class RouteReportRequest
{
    public List<ReportSource> ReportSource { get; set; }
    public List<string> Interaction { get; set; }
    public List<string> Workflow { get; set; }
    public bool IsWorkflow => !Interaction.Any() && Workflow.Any();
    public bool IsInteraction => Interaction.Any() && !Workflow.Any();
    public string? ReportName { get; set; } = null;
    public Guid? MessageGroupId { get; set; } = Guid.NewGuid();
}