using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System.Collections.Generic;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        List<OrganisationList> GetOrganisationDetailsByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
        Organisation GetOrganisationDetailsByOdsCode(string odsCode);

        List<SpineList> GetGpProviderEndpointAndPartyKeyByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
        List<SpineList> GetGpProviderAsIdByOdsCodeAndPartyKey(List<SpineList> odsCodeWithPartyKey);
        
        Spine GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode);
        Spine GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey);
        Spine GetGpConsumerAsIdByOdsCode(string odsCode);
        List<SpineList> GetGpConsumerAsIdByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
    }
}
