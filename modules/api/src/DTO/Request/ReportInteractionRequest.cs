namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportInteractionRequest
{
    public List<ReportSource> ReportSource { get; set; }
    public string? InteractionId { get; set; } = null;
    public string? ReportName { get; set; } = null;
    public Guid MessageGroupId { get; set; }    
}