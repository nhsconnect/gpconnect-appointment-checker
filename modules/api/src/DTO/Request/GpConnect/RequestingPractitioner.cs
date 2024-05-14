using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestingPractitioner : BaseRequest
{
    [JsonProperty("name")]
    public List<Name> name { get; set; }
}

public class Name
{
    [JsonProperty("family")]
    public string family { get; set; }
    [JsonProperty("given")]
    public List<string> given { get; set; }
}