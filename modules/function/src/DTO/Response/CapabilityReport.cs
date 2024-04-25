using GpConnect.AppointmentChecker.Function.Helpers;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class CapabilityReport
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("interactions")]
    public List<string> Interaction { get; set; }

    [JsonProperty("workflows")]
    public List<string> Workflow { get; set; }

    public string ObjectKey => $"{ReportName.ReplaceNonAlphanumeric()}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}.json".ToLower();

}
