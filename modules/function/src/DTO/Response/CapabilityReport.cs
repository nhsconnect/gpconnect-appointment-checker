using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class CapabilityReport
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("interactionId")]
    public string InteractionId { get; set; }
}
