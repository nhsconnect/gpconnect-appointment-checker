using DocumentFormat.OpenXml.EMMA;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models.Search;

public class SearchResultList
{
    [JsonProperty("searchResultsTotalCount")]
    public int SearchResultsTotalCount { get; set; }

    [JsonProperty("searchResultsCurrentCount")]
    public int SearchResultsCurrentCount { get; set; }

    [JsonProperty("searchResultsPastCount")]
    public int SearchResultsPastCount { get; set; }

    [JsonProperty("timeTaken")]
    public TimeSpan TimeTaken { get; set; }

    public string SearchStats => string.Format(SearchConstants.SEARCHSTATSTEXT, TimeTaken.ToString("#.##s"), DateTime.Now.TimeZoneConverter("Europe/London", "d MMM yyyy HH:mm:ss"));

    [JsonProperty("searchResultsPast")]
    public List<SearchResultEntry> SearchResultsPast { get; set; }

    [JsonProperty("searchResults")]
    public List<SearchResultEntry> SearchResults { get; set; }

    [JsonProperty("currentSlotEntriesByLocationGrouping")]
    public List<List<SearchResultEntry>> CurrentSlotEntriesByLocationGrouping { get; set; }

    [JsonProperty("pastSlotEntriesByLocationGrouping")]
    public List<List<SearchResultEntry>> PastSlotEntriesByLocationGrouping { get; set; }

    [JsonProperty("providerOdsCodeFound")]
    public bool ProviderOdsCodeFound { get; set; }

    [JsonProperty("consumerOdsCodeFound")]
    public bool ConsumerOdsCodeFound { get; set; }

    [JsonProperty("providerEnabledForGpConnectAppointmentManagement")]
    public bool ProviderEnabledForGpConnectAppointmentManagement { get; set; }

    [JsonProperty("consumerEnabledForGpConnectAppointmentManagement")]
    public bool ConsumerEnabledForGpConnectAppointmentManagement { get; set; }

    [JsonProperty("providerASIDPresent")]
    public bool ProviderASIDPresent { get; set; }

    [JsonProperty("capabilityStatementOk")]
    public bool CapabilityStatementOk { get; set; }

    [JsonProperty("slotSearchOk")]
    public bool SlotSearchOk { get; set; }

    [JsonProperty("formattedProviderOrganisationDetails")]
    public string FormattedProviderOrganisationDetails { get; set; }

    [JsonProperty("formattedConsumerOrganisationDetails")]
    public string FormattedConsumerOrganisationDetails { get; set; }

    [JsonProperty("formattedConsumerOrganisationType")]
    public string FormattedConsumerOrganisationType { get; set; }

    [JsonProperty("providerPublisher")]
    public string ProviderPublisher { get; set; }

    [JsonProperty("providerError")]
    public string ProviderError { get; set; }
}
