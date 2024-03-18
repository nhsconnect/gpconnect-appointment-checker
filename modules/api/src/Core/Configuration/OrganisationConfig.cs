namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class OrganisationConfig
{
    public string BaseFhirApiUrl { get; set; } = "";
    public string BaseOdsApiUrl { get; set; } = "";
    public string HierarchyOdsApiUrl { get; set; } = "";
    public string HierarchyOdsApiClientId { get; set; } = "";
    public string HierarchyOdsApiClientSecret { get; set; } = "";
    public string HierarchyFhirBaseUrl { get; set; } = "";
    public string HierarchyOdsBaseUrl { get; set; } = "";
    public int RecordLimit { get; set; }
}