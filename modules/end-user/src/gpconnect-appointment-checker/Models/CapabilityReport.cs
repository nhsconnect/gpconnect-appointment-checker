using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class CapabilityReport
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("interactionId")]
    public string InteractionId { get; set; }
}
