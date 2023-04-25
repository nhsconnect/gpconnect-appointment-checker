using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class Spine
{
    [JsonProperty("useSSP")] 
    public bool UseSSP { get; set; }
    [JsonProperty("sspHostname")]
    public string SspHostname { get; set; }
    [JsonProperty("endpointAddress")]
    public string EndpointAddress { get; set; }
    [JsonProperty("sdsHostname")]
    public string SdsHostname { get; set; }
    [JsonProperty("clientCert")]
    public string ClientCert { get; set; }
    [JsonProperty("clientPrivateKey")]
    public string ClientPrivateKey { get; set; }
    [JsonProperty("serverCACertChain")]
    public string ServerCACertChain { get; set; }
    [JsonProperty("sdsPort")]
    public int SdsPort { get; set; }
    [JsonProperty("sdsUseLdaps")]
    public bool SdsUseLdaps { get; set; }
    [JsonProperty("sdsUseMutualAuth")]
    public bool SdsUseMutualAuth { get; set; }
    [JsonProperty("organisationId")]
    public int OrganisationId { get; set; }
    [JsonProperty("odsCode")]
    public string OdsCode { get; set; }
    [JsonProperty("manufacturingOrganisationOdsCode")]
    public string ManufacturingOrganisationOdsCode { get; set; }
    [JsonProperty("organisationName")]
    public string OrganisationName { get; set; }
    [JsonProperty("partyKey")]
    public string PartyKey { get; set; }
    [JsonProperty("asId")]
    public string AsId { get; set; }
    [JsonProperty("sspFrom")]
    public string SspFrom { get; set; }
    [JsonProperty("hasAsId")]
    public bool HasAsId { get; set; }
    [JsonProperty("timeoutSeconds")]
    public int TimeoutSeconds { get; set; }
    [JsonProperty("timeoutMilliseconds")]
    public int TimeoutMilliseconds { get; set; }
    [JsonProperty("spineFqdn")]
    public string SpineFqdn { get; set; }
    [JsonProperty("sdsTlsVersion")]
    public string SdsTlsVersion { get; set; }
    [JsonProperty("productName")]
    public string ProductName { get; set; }
    [JsonProperty("sdsUseFhirApi")]
    public bool SdsUseFhirApi { get; set; }
    [JsonProperty("spineFhirApiSystemsRegisterFqdn")]
    public string SpineFhirApiSystemsRegisterFqdn { get; set; }
    [JsonProperty("spineFhirApiDirectoryServicesFqdn")]
    public string SpineFhirApiDirectoryServicesFqdn { get; set; }
    [JsonProperty("spineFhirApiKey")]
    public string SpineFhirApiKey { get; set; }
}
