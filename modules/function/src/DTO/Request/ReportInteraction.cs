using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;

    public string InteractionKey => $"{ ReportName?.ReplaceNonAlphanumeric() }_{ InteractionId?.ReplaceNonAlphanumeric() }_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}.xlsx".ToLower();

    public string? ReportName { get; set; } = null;
}
