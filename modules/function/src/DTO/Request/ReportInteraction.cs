using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<ReportSource> ReportSource { get; set; } = null;
    public string? InteractionId { get; set; } = null;    
    public string InteractionKeyJson => $"{Helpers.Constants.Objects.Transient}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}_{ Guid.NewGuid() }_{ ReportName?.ReplaceNonAlphanumeric() }_{ InteractionId?.ReplaceNonAlphanumeric() }.json".ToLower();
    public string? ReportName { get; set; } = null;
    public string? PreSignedUrl { get; set; } = null;
}
