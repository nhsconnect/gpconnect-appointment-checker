using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class General
{
    [JsonProperty("productName")]
    public string ProductName { get; set; }

    [JsonProperty("productVersion")]
    public string ProductVersion { get; set; }

    [JsonProperty("maxNumWeeksSearch")]
    public int MaxNumWeeksSearch { get; set; }

    [JsonProperty("maxNumberProviderCodesSearch")]
    public int MaxNumberProviderCodesSearch { get; set; }

    [JsonProperty("maxNumberConsumerCodesSearch")]
    public int MaxNumberConsumerCodesSearch { get; set; }
    
    [JsonProperty("logRetentionDays")]
    public int LogRetentionDays { get; set; }

    [JsonProperty("getAccessEmailAddress")]
    public string GetAccessEmailAddress { get; set; }
}
