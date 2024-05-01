using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

public class CapabilityReport
{
    public string ReportName { get; set; }
    public string ReportId { get; set; }
    protected string? Interaction { get; private set; }
    protected string? Workflow { get; private set; }
    public List<string>? Workflows => Workflow != null ? JsonConvert.DeserializeObject<List<string>>(Workflow) : null;
    public List<string>? Interactions => Interaction != null ? JsonConvert.DeserializeObject<List<string>>(Interaction) : null;
}
