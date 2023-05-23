using GpConnect.AppointmentChecker.Models.Search;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models;

public class SlotEntrySummary
{
    public string ProviderOdsCode { get; set; }
    public string ConsumerOdsCode { get; set; }
    public string FormattedProviderOrganisationDetails { get; set; }
    public string FormattedConsumerOrganisationDetails { get; set; }
    [JsonProperty("consumerOrganisationType")]
    public string ConsumerOrganisationType { get; set; }
    [JsonProperty("searchSummaryDetail")]
    public List<string> SearchSummaryDetail { get; set; }
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
