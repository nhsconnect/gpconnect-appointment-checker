using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestingPractitioner : BaseRequest
{
    [JsonProperty("name")]
    public List<Name> name { get; set; }
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("practitionerRole")]
    public PractitionerRole practitionerRole { get; set; }
}

public class PractitionerRole
{
    [JsonProperty("role")]
    public Role role { get; set; }
}

public class Role
{
    [JsonProperty("coding")]
    public List<Coding> coding { get; set; }
}

public class Coding
{
    [JsonProperty("system")]
    public string system { get; set; }
    [JsonProperty("code")]
    public string code { get; set; }
}

public class Name
{
    [JsonProperty("family")]
    public string family { get; set; }
    [JsonProperty("given")]
    public List<string> given { get; set; }
}
