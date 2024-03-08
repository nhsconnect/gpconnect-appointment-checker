namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportCreationRequest
{
    public string JsonData { get; set; }
    public string ReportName { get; set; }
    public List<string> ReportTabs { get; set; } = new List<string>();
}