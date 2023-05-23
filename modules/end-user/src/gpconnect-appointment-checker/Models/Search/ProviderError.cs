using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models.Search;

public class ProviderError
{
    [JsonProperty("display")] 
    public string Display { get; set; }
    [JsonProperty("code")]
    public string Code { get; set; }
    [JsonProperty("diagnostics")]
    public string Diagnostics { get; set; }
}
