using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class TransientData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("data")]
    public string Data { get; set; }

    [JsonProperty("reportId")]
    public string ReportId { get; set; }

    [JsonProperty("reportName")]
    public string ReportName { get; set; }

}
