using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Detail
{
    [JsonProperty("coding")]
    public List<Coding> Coding { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
}
