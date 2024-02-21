namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportInteractionRequest
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;
    public string? ReportName { get; set; } = null;
    public Guid MessageGroupId { get; set; }    
}