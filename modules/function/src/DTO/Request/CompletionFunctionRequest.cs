namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class CompletionFunctionRequest
{
    public List<string> DistributionList { get; set; }
    public string? ReportName { get; set; }
    public List<ReportFilterRequest> ReportFilter { get; set; }
}
