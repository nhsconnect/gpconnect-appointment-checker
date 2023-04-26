using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestingOrganisation : BaseRequest
{
    [JsonProperty("name")]
    public string name { get; set; }
}
