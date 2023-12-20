using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Operation
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("definition")]
    public Definition Definition { get; set; }
}