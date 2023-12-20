using Newtonsoft.Json;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models;

public class SearchResult
{
    [JsonProperty("searchGroupId")]
    public int SearchGroupId { get; set; }
    [JsonProperty("searchResultId")]
    public int SearchResultId { get; set; }

    [JsonProperty("responsePayload")]
    public string ResponsePayload { get; set; }

    [JsonProperty("providerOdsCode")]
    public string ProviderOdsCode { get; set; }

    [JsonProperty("consumerOdsCode")]
    public string ConsumerOdsCode { get; set; }

    [JsonProperty("providerOrganisationName")]
    public string ProviderOrganisationName { get; set; }

    [JsonProperty("providerAddress")]
    public string ProviderAddress { get; set; }

    [JsonProperty("providerAddressFields")]
    public List<string> ProviderAddressFields { get; set; }

    [JsonProperty("providerPostcode")]
    public string ProviderPostcode { get; set; }

    [JsonProperty("consumerOrganisationName")]
    public string ConsumerOrganisationName { get; set; }

    [JsonProperty("consumerAddress")]
    public string ConsumerAddress { get; set; }

    [JsonProperty("consumerAddressFields")]
    public List<string> ConsumerAddressFields { get; set; }

    [JsonProperty("consumerPostcode")]
    public string ConsumerPostcode { get; set; }

    [JsonProperty("providerPublisher")]
    public string ProviderPublisher { get; set; }

    [JsonProperty("searchDurationSeconds")]
    public double SearchDurationSeconds { get; set; }

    [JsonProperty("consumerOrganisationType")]
    public string ConsumerOrganisationType { get; set; }

    [JsonProperty("searchAtResults")]
    public string SearchAtResults { get; set; }

    [JsonProperty("searchOnBehalfOfResults")]
    public string SearchOnBehalfOfResults { get; set; }
}
