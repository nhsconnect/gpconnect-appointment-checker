using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Profile
{
    [JsonProperty("Reference")]
    public string reference { get; set; }
}