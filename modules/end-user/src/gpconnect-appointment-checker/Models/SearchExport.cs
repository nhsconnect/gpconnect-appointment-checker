using Newtonsoft.Json;
using System;

namespace GpConnect.AppointmentChecker.Models;

public class SearchExport
{
    [JsonProperty("searchExportId")]
    public int SearchExportId { get; set; }
}
