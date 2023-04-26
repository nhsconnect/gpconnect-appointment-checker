using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class SearchResult
{
    [JsonProperty("searchGroupId")]
    public int SearchGroupId { get; set; }
    [JsonProperty("searchResultId")]
    public int SearchResultId { get; set; }
}
