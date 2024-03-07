using GpConnect.AppointmentChecker.Function.Helpers;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class CapabilityReport
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("interactions")]
    public List<string> Interaction { get; set; }

    public string InteractionKey => $"{Helpers.Constants.Objects.Interaction}_{DateTime.Now.ToString("s").ReplaceNonAlphanumeric()}.json".ToLower();

}
