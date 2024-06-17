namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class OrganisationConfig
{
    public string BaseFhirApiUrl { get; set; } = "";
    public int RecordLimit { get; set; }
}