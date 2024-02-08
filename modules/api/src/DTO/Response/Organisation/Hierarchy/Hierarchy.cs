using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

public class Hierarchy
{
    [JsonProperty("ODS Code")] 
    public string OdsCode { get; set; }
    [JsonProperty("Site Name")]
    public string SiteName { get; set; }
    [JsonProperty("Postcode")]
    public string Postcode { get; set; }
    [JsonProperty("ICB Code")]
    public string? IcbCode { get; set; }
    [JsonProperty("Higher Health Authority Code")]
    public string? HigherHealthAuthorityCode { get; set; }
    [JsonProperty("Commissioning Region Code")] 
    public string? NationalGroupingCode { get; set; }
    [JsonProperty("ICB Name")]
    public string? IcbName { get; set; }
    [JsonProperty("Higher Health Authority Name")]
    public string? HigherHealthAuthorityName { get; set; }
    [JsonProperty("Commissioning Region Name")]
    public string? NationalGroupingName { get; set; }
}