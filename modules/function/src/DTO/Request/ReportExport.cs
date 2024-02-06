namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportExport
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;
    public string? ReportName { get; set; } = null;
}
