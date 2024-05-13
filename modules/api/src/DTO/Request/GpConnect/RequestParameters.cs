using GpConnect.AppointmentChecker.Api.Helpers.Constants;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestParameters
{
    public Uri RequestUri { get; set; }
    public SpineProviderRequestParameters ProviderSpineDetails { get; set; }
    public OrganisationRequestParameters ProviderOrganisationDetails { get; set; }
    public OrganisationRequestParameters ConsumerOrganisationDetails { get; set; }
    public SpineMessageTypes SpineMessageTypeId { get; set; }
    public string Sid { get; set; }
    public string ConsumerOrganisationType { get; set; }
    public string SystemIdentifier { get; set; } = "https://fhir.nhs.uk/Id/ods-organization-code";
    public string HostIdentifier { get; set; } = "https://fhir.nhs.uk";
    public string? AuthenticationAudience { get; set; } = null;
}
