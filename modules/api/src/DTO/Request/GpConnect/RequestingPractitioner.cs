using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestingPractitioner : BaseRequest
{
    [JsonProperty("name")]
    public List<Name> name { get; set; }
    [JsonProperty("id")]
    public string id { get; set; }
    [JsonProperty("practitionerRole")]
    public PractitionerRole PractitionerRole { get; set; }
}

public class PractitionerRole
{
    [JsonProperty("role")]
    public Role Role { get; set; }
}

public class Role
{
    [JsonProperty("coding")]
    public List<Coding> Coding { get; set; }
}

public class Coding
{
    [JsonProperty("system")]
    public string System { get; set; }
    [JsonProperty("code")]
    public string Code { get; set; }
}

public class Name
{
    [JsonProperty("family")]
    public string family { get; set; }
    [JsonProperty("given")]
    public List<string> given { get; set; }
}
