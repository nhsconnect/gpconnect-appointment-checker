namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportCreationRequest
{
    public string ReportName { get; set; }
    public string ReportId { get; set; }
    public List<ReportFilterRequest>? ReportFilter { get; set; } = null;
}