using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Contact
{
    [JsonProperty("name")]
    public string Name { get; set; }
}