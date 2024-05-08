using GpConnect.AppointmentChecker.Function.Helpers;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportCreationRequest
{
    public string JsonData { get; set; }
    public string ReportName { get; set; }
    public string ReportId { get; set; }
    public string ReportKey => $"{Helpers.Constants.Objects.GpConnect}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}_{ReportName?.ReplaceNonAlphanumeric()}.xlsx".ToLower();
    public List<ReportFilterRequest> ReportFilter { get; set; }
    public byte[]? ReportBytes { get; set; } = null;
}
