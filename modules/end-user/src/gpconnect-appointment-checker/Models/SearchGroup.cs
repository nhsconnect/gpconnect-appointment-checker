using Newtonsoft.Json;
using System;

namespace GpConnect.AppointmentChecker.Models;

public class SearchGroup
{
    [JsonProperty("searchGroupId")]
    public int SearchGroupId { get; set; }
    [JsonProperty("providerOdsTextbox")]
    public string ProviderOdsTextbox { get; set; }
    [JsonProperty("consumerOdsTextbox")]
    public string ConsumerOdsTextbox { get; set; }
    [JsonProperty("selectedDateRange")]
    public string SelectedDateRange { get; set; }
    [JsonProperty("consumerOrganisationTypeDropdown")]
    public string ConsumerOrganisationTypeDropdown { get; set; }
    [JsonProperty("searchStartAt")]
    public DateTime SearchStartAt { get; set; }
    [JsonProperty("searchEndAt")]
    public DateTime? SearchEndAt { get; set; }
}
