using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        List<OrganisationList> GetOrganisationDetailsByOdsCode(List<string> odsCode);
        List<SpineList> GetGpProviderEndpointAndPartyKeyByOdsCode(List<string> odsCode);
        List<SpineList> GetGpProviderAsIdByOdsCodeAndPartyKey(List<SpineList> odsCodeWithPartyKey);
    }
}
