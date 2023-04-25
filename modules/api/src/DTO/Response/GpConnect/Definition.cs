using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Definition
{
    [JsonProperty("reference")]
    public string Reference { get; set; }
}