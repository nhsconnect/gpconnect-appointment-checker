namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class SpineConfig
{
    public bool UseSSP { get; set; }
    public string SspHostname { get; set; }
    public string EndpointAddress { get; set; }
    public string SdsHostname { get; set; }
    public string ClientCert { get; set; }
    public string ClientPrivateKey { get; set; }
    public string ServerCACertChain { get; set; }
    public int SdsPort { get; set; }
    public bool SdsUseLdaps { get; set; }
    public bool SdsUseMutualAuth { get; set; }
    public int OrganisationId { get; set; }
    public string OdsCode { get; set; }
    public string ManufacturingOrganisationOdsCode { get; set; }
    public string OrganisationName { get; set; }
    public string PartyKey { get; set; }
    public string AsId { get; set; }
    public string SspFrom { get; set; }
    public bool HasAsId => !string.IsNullOrEmpty(AsId);
    public int TimeoutSeconds { get; set; }
    public int TimeoutMilliseconds => TimeoutSeconds * 1000;
    public string SpineFqdn { get; set; }
    public string SdsTlsVersion { get; set; }
    public string ProductName { get; set; }
    public bool SdsUseFhirApi { get; set; }
    public string SpineFhirApiSystemsRegisterFqdn { get; set; }
    public string SpineFhirApiDirectoryServicesFqdn { get; set; }
    public string SpineFhirApiKey { get; set; }
    public string DefaultInteraction { get; set; }
}