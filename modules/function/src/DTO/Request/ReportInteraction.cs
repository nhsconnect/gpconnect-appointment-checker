using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;

    public string InteractionKey => $"gpconnect_{ ReportName?.ReplaceNonAlphanumeric() }_{ InteractionId?.ReplaceNonAlphanumeric() }_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}.xlsx".ToLower();
    public string InteractionKeyJson => $"transient_{ Guid.NewGuid() }_{ ReportName?.ReplaceNonAlphanumeric() }_{ InteractionId?.ReplaceNonAlphanumeric() }_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}.json".ToLower();

    public string? ReportName { get; set; } = null;
    public string? PreSignedUrl { get; set; } = null;
}
