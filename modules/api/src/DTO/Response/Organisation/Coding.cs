using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class Coding
{
    [JsonProperty("system")]
    public string System { get; set; }
    [JsonProperty("code")]
    public string Code { get; set; }
    [JsonProperty("display")]
    public string Display { get; set; }
}