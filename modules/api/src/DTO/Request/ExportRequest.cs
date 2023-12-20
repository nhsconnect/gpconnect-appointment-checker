namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ExportRequest
{
    public int ExportRequestId { get; set; }
    public int UserId { get; set; }
    public string ReportName { get; set; }
}