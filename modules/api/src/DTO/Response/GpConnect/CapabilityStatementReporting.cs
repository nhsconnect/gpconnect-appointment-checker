using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class CapabilityStatementReporting
{
    public string OdsCode { get; set; }
    public string Version { get; set; }
    public List<Profile> Profile { get; set; }
    [JsonProperty("Operation")] 
    public IEnumerable<IEnumerable<string>> Rest { get; set; }
}