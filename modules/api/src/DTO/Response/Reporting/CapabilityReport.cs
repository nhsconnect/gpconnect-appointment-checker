using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;

public class CapabilityReport
{
    public string ReportName { get; set; }
    protected string Interaction { get; private set; }
    public List<string> Interactions => JsonConvert.DeserializeObject<List<string>>(Interaction);

}
