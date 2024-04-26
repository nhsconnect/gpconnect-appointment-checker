using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Mesh;

public class Result
{
    [JsonProperty("endpoint_type")]
    public string EndPointType { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("address")]
    public string Address { get; set; }
}