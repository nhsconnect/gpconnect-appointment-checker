using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        Organisation GetOrganisationDetailsByOdsCode(string odsCode);
        Spine GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode);
        Spine GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey);
    }
}
