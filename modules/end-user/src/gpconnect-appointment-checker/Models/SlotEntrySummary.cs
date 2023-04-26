using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class SlotEntrySummary
{
    [JsonProperty("providerLocationName")]
    public string ProviderLocationName { get; set; }
    [JsonProperty("providerOdsCode")]
    public string ProviderOdsCode { get; set; }
    [JsonProperty("consumerLocationName")]
    public string ConsumerLocationName { get; set; }
    [JsonProperty("consumerOdsCode")]
    public string ConsumerOdsCode { get; set; }
    [JsonProperty("consumerOrganisationType")]
    public string ConsumerOrganisationType { get; set; }
    [JsonProperty("searchSummaryDetail")]
    public string SearchSummaryDetail { get; set; }
    [JsonProperty("searchSummaryAdditionalDetail")]
    public string SearchSummaryAdditionalDetail { get; set; }
    [JsonProperty("providerPublisher")]
    public string ProviderPublisher { get; set; }
    [JsonProperty("searchResultId")]
    public int SearchResultId { get; set; }
    [JsonProperty("searchGroupId")]
    public int SearchGroupId { get; set; }
    [JsonProperty("detailsEnabled")]
    public bool DetailsEnabled { get; set; }
    [JsonProperty("displayProvider")]
    public bool DisplayProvider { get; set; }
    [JsonProperty("displayConsumer")]
    public bool DisplayConsumer { get; set; }
    [JsonProperty("displayClass")]
    public string DisplayClass { get; set; }
    [JsonProperty("displayConsumerOrganisationType")]
    public bool DisplayConsumerOrganisationType { get; set; }
}
