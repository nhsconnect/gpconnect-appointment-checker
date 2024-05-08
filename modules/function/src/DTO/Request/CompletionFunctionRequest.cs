namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class CompletionFunctionRequest
{    
    public DistributionListRequest DistributionList { get; set; }
    public List<ReportFilterRequest> ReportFilter { get; set; }
}
