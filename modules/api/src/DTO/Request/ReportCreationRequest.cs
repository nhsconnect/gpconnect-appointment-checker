namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportCreationRequest
{
    public string JsonData { get; set; }
    public string ReportName { get; set; }
    public List<ReportFilterRequest>? ReportFilter { get; set; } = null;
}