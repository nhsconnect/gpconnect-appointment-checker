using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Software
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("version")]
    public string Version { get; set; }
    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }
}