using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestParameters
{
    public Uri RequestUri { get; set; }
    public SpineProviderRequestParameters ProviderSpineDetails { get; set; }
    public OrganisationRequestParameters ProviderOrganisationDetails { get; set; }
    //public SpineConsumerRequestParameters ConsumerEnablement { get; set; }
    public OrganisationRequestParameters ConsumerOrganisationDetails { get; set; }
    public SpineMessageTypes SpineMessageTypeId { get; set; }
    public User UserDetails { get; set; }
    public string Sid { get; set; }
    public string ConsumerOrganisationType { get; set; }
}
