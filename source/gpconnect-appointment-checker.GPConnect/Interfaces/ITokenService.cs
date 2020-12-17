using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface ITokenService
    {
        RequestParameters ConstructRequestParameters(Uri requestUri, Spine providerSpineMessage, Organisation providerOrganisationDetails, Spine consumerSpineMessage, Organisation consumerOrganisationDetails, int spineMessageTypeId);
    }
}
