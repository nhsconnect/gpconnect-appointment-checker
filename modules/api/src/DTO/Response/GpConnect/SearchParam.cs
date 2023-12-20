using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class SearchParam
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("documentation")]
    public string Documentation { get; set; }
}