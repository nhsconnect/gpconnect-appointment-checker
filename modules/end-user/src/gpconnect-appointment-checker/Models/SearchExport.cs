using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class SearchExport
{
    [JsonProperty("searchExportId")]
    public int SearchExportId { get; set; }
}
