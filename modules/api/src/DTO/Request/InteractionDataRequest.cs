using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class InteractionDataRequest
{
    public string Interaction { get; set; }
    public string OdsCode { get; set; }
    public string Client { get; set; }
    public string SystemIdentifier => $"{HostIdentifier}/Id/ods-organization-code";
    public string HostIdentifier { get; set; }
    public string? AuthenticationAudience { get; set; } = null;
    public bool HasId { get; set; } = true;
}