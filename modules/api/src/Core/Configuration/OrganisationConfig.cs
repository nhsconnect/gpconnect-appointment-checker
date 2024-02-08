namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class OrganisationConfig
{
    public string BaseFhirApiUrl { get; set; } = "";
    public string BaseOdsApiUrl { get; set; } = "";
    public string HierarchyOdsApiUrl { get; set; } = "";
    public string HierarchyOdsApiClientId { get; set; } = "";
    public string HierarchyOdsApiClientSecret { get; set; } = "";
}