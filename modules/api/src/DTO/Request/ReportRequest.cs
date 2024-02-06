namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportRequest
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;
    public string? FunctionName { get; set; } = null;
    public string? ReportName { get; set; } = null;
}