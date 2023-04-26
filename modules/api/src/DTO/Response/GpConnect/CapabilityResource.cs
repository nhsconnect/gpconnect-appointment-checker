using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class CapabilityResource
{
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("interaction")]
    public List<Interaction> Interaction { get; set; }
    [JsonProperty("searchParam")]
    public List<SearchParam> SearchParam { get; set; }
    [JsonProperty("updateCreate")]
    public bool? UpdateCreate { get; set; }
    [JsonProperty("searchInclude")]
    public List<string> SearchInclude { get; set; }
}