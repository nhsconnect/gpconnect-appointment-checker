using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class CapabilityReport
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("reportId")]
    public string ReportId { get; set; }

    [JsonProperty("interactions")]
    public List<string> Interaction { get; set; }

    [JsonProperty("workflows")]
    public List<string> Workflow { get; set; }

    public string ObjectKey => $"{Objects.Key}_{ReportName.ReplaceNonAlphanumeric()}.json".ToLower();

    public Guid MessageGroupId => Guid.NewGuid();
}
