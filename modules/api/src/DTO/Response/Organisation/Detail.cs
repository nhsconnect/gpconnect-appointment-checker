using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class Detail
{
    [JsonProperty("coding")]
    public Coding Coding { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
}