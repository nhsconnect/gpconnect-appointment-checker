using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;

    public string InteractionKey => $"{ InteractionId?.ReplaceNonAlphanumeric() }_{DateTime.UtcNow.ToFileTimeUtc}";

    public string? ReportName { get; set; } = null;
}
