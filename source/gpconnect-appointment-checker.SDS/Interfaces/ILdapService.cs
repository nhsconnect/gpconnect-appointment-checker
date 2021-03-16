using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System.Collections.Generic;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        List<OrganisationList> GetOrganisationDetailsByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
        List<SpineList> GetGpProviderEndpointAndPartyKeyByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
        List<SpineList> GetGpProviderAsIdByOdsCodeAndPartyKey(List<SpineList> odsCodeWithPartyKey);
        Organisation GetOrganisationDetailsByOdsCode(string odsCode);
        Spine GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode);
        Spine GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey);
    }
}
