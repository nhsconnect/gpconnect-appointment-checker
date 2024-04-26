using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportInteraction
{
    public List<ReportSource> ReportSource { get; set; } = null;
    public List<string>? Interaction { get; set; }
    public List<string>? Workflow { get; set; }
    public string ObjectKeyJson => $"{Helpers.Constants.Objects.Transient}_{ReportName?.ReplaceNonAlphanumeric()}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}_{GetTypeLabel(Interaction, Workflow)}_{ Guid.NewGuid() }.json".ToLower();

    private string GetTypeLabel(List<string>? interaction, List<string>? workflow)
    {
        var typeLabel = string.Empty;
        if (interaction != null && interaction[0] != null)
        {
            typeLabel = interaction[0].ReplaceNonAlphanumeric();
        }
        if (workflow != null && workflow[0] != null)
        {
            typeLabel = workflow[0].ReplaceNonAlphanumeric();
        }
        return typeLabel;
    }

    public string? ReportName { get; set; } = null;
    public string? PreSignedUrl { get; set; } = null;
}
