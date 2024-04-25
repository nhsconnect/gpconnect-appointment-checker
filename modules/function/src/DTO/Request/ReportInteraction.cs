using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<ReportSource> ReportSource { get; set; } = null;
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string ObjectKeyJson => $"{Helpers.Constants.Objects.Transient}_{ReportName?.ReplaceNonAlphanumeric()}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}_{ Guid.NewGuid() }_{Interaction[0]?.ReplaceNonAlphanumeric() }.json".ToLower();
    public string? ReportName { get; set; } = null;
    public string? PreSignedUrl { get; set; } = null;
}
