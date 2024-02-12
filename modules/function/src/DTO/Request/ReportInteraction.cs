namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;

    public string InteractionKey => InteractionId?.Insert(InteractionId.Length+1, DateTime.UtcNow.ToOADate().ToString());

    public string? ReportName { get; set; } = null;
}
