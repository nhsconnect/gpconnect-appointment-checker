namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class FunctionRequest
{
    public List<string> DistributionList { get; set; }
    public List<string> OdsCodes { get; set; } = new List<string>();
    public string ReportName { get; set; }
}
