namespace GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
public class SdsQuery : QueryBase
{
    public string SearchBase { get; set; }
    public string QueryAttributes { get; set; }
    public string[] QueryAttributesAsArray => QueryAttributes?.Replace(" ", "").Split(",");
}
