using Newtonsoft.Json;
using System;

namespace GpConnect.AppointmentChecker.Models;

public class SearchGroupExport
{
    [JsonProperty("searchGroupId")]
    public int SearchGroupId { get; set; }
    [JsonProperty("searchResultId")]
    public int SearchResultId { get; set; }

    [JsonProperty("providerOdsCode")]
    public string ProviderOdsCode { get; set; }
    [JsonProperty("consumerOdsCode")]
    public string ConsumerOdsCode { get; set; }
    [JsonProperty("providerOrganisationName")]
    public string ProviderOrganisationName { get; set; }
    [JsonProperty("providerAddress")]
    public string ProviderAddress { get; set; }
    [JsonProperty("providerPostcode")]
    public string ProviderPostcode { get; set; }
    [JsonProperty("consumerOrganisationName")]
    public string ConsumerOrganisationName { get; set; }
    [JsonProperty("consumerAddress")]
    public string ConsumerAddress { get; set; }
    [JsonProperty("consumerPostcode")]
    public string ConsumerPostcode { get; set; }
    [JsonProperty("errorCode")]
    public int errorCode { get; set; }
    [JsonProperty("details")]
    public string Details { get; set; }
    [JsonProperty("providerPublisher")]
    public string ProviderPublisher { get; set; }
    [JsonProperty("consumerOrganisationType")]
    public string ConsumerOrganisationType { get; set; }
}
