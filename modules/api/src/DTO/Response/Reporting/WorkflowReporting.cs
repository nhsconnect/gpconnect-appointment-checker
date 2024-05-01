using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public class WorkflowReporting : InteractionReporting
{
    [JsonProperty("Status")]
    public string Status { get; set; }
}