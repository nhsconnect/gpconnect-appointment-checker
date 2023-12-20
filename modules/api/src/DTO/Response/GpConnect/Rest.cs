using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Rest
{
    [JsonProperty("mode")]
    public string Mode { get; set; }
    [JsonProperty("security")]
    public Security Security { get; set; }
    [JsonProperty("resource")]
    public List<CapabilityResource> Resource { get; set; }
    [JsonProperty("operation")]
    public List<Operation> Operation { get; set; }
}
