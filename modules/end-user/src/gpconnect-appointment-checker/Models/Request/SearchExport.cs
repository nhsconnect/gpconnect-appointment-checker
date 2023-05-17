namespace GpConnect.AppointmentChecker.Models.Request;

public class SearchExport
{
    public int UserId { get; set; }
    public int ExportRequestId { get; set; }
    public string ReportName { get; set; }
}
