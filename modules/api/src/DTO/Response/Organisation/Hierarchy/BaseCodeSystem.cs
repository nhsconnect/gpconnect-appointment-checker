using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

public class Parameter
{
    [JsonProperty("part")]
    public List<Part?>? Part { get; set; }
}

public class Part
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";
    [JsonProperty("valueCode")]
    public string ValueCode { get; set; } = "";
    [JsonProperty("valueString")]
    public string ValueString { get; set; } = "";
}
internal class BaseCodeSystem
{
    [JsonProperty("parameter")]
    public List<Parameter?>? Parameter { get; set; }
}
