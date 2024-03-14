using GpConnect.AppointmentChecker.Api.Helpers;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class CapabilityStatement
{
    [JsonProperty("resourceType")]
    public string ResourceType { get; set; }
    [JsonProperty("version")]
    public string Version { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("Status")]
    public string status { get; set; }
    [JsonProperty("date")]
    public string Date { get; set; }
    [JsonProperty("publisher")]
    public string Publisher { get; set; }
    [JsonProperty("contact")]
    public List<Contact> Contact { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("copyright")]
    public string Copyright { get; set; }
    [JsonProperty("kind")]
    public string Kind { get; set; }
    [JsonProperty("software")]
    public Software Software { get; set; }
    [JsonProperty("fhirVersion")]
    public string FhirVersion { get; set; }
    [JsonProperty("acceptUnknown")]
    public string AcceptUnknown { get; set; }
    [JsonProperty("format")]
    public List<string> Format { get; set; }
    [JsonProperty("profile")]
    public List<Profile> Profile { get; set; }
    [JsonProperty("rest")]
    public List<Rest> Rest { get; set; }

    [JsonProperty("issue")]
    public List<Issue?> Issue { get; set; } = new List<Issue?>();

    public bool NoIssues => Issue?.Count == 0 || Issue == null;

    public string ProviderError => Issue?.FirstOrDefault()?.Details.Coding?.FirstOrDefault()?.Display;
    public string ProviderErrorCode => Issue?.FirstOrDefault()?.Details.Coding?.FirstOrDefault()?.Code;
    public string ProviderErrorDiagnostics => StringExtensions.Coalesce(Issue?.FirstOrDefault()?.Diagnostics, Issue?.FirstOrDefault()?.Details.Text);
}