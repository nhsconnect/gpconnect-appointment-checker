namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class RouteReportRequest
{
    public List<ReportSource> ReportSource { get; set; }
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    //public bool IsInteraction => Interaction != null && Interaction.Any() && Workflow == null;
    public string? ReportName { get; set; } = null;
    public string? ReportId { get; set; } = null;
    //public Guid? MessageGroupId { get; set; } = Guid.NewGuid();
}