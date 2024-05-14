namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class ReportingRequestingDevice : BaseRequest
{
    public string model { get; set; }
    public string version { get; set; }
    public string id { get; set; }
}
