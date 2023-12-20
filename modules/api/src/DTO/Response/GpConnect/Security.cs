using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Security
{
    [JsonProperty("cors")]
    public bool Cors { get; set; }
}