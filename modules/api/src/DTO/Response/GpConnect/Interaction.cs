using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Interaction
{
    [JsonProperty("code")]
    public string Code { get; set; }
}