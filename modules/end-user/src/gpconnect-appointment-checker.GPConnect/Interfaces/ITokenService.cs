using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface ITokenService
    {
        Task<List<RequestParametersList>> ConstructRequestParameters(Uri requestUri, List<SpineList> providerSpineMessage, List<OrganisationList> providerOrganisationDetails, List<SpineList> consumerSpineMessage, List<OrganisationList> consumerOrganisationDetails, int spineMessageTypeId, string consumerOrganisationType);
        RequestParameters ConstructRequestParameters(Uri requestUri, Spine providerSpineMessage, Organisation providerOrganisationDetails, Spine consumerEnablement, Organisation consumerOrganisationDetails, int spineMessageTypeId, string consumerOrganisationType);
    }
}
