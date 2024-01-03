using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class RequestParameters
{
    public string BearerToken { get; set; }
    public string SspFrom { get; set; }
    public string SspTo { get; set; }
    public string EndpointAddress { get; set; }
    public string SspHostname { get; set; }
    public bool UseSSP { get; set; }
    public string ProviderODSCode { get; set; }
    public string ConsumerODSCode { get; set; }
    public string InteractionId { get; set; }
    public SpineMessageTypes SpineMessageTypeId { get; set; }
    public int RequestTimeout { get; set; }
    public string GPConnectConsumerOrganisationType { get; set; }

    public string EndpointAddressWithSpineSecureProxy => AddSpineSecureProxy();

    private string AddSpineSecureProxy()
    {
        return UriExtensions.CheckForTrailingSlash(UseSSP ? $"{SspHostname.AddScheme()}/{EndpointAddress}" : EndpointAddress);
    }
}
