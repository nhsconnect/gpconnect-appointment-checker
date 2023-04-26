using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class Report
{
    [JsonProperty("reportName")]
    public string ReportName { get; set; }

    [JsonProperty("functionName")]
    public string FunctionName { get; set; }
}
