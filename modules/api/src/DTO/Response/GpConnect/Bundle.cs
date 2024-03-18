using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class Bundle
{
    public string resourceType { get; set; }
    public string id { get; set; }
    public RootMeta meta { get; set; }
    public string type { get; set; }
    public List<RootLink> link { get; set; }
    public List<RootEntry> entry { get; set; }
    [JsonProperty("issue")]
    public List<Issue> Issue { get; set; }
}