using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class Issue
{
    [JsonProperty("severity")]
    public string Severity { get; set; }
    [JsonProperty("code")]
    public string Code { get; set; }
    [JsonProperty("details")]
    public Detail Details { get; set; }
    [JsonProperty("diagnostics")]
    public string Diagnostics { get; set; }
}