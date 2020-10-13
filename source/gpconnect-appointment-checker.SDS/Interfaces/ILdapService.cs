using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode);
        Task<Spine> GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode);
        Task<Spine> GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey);
    }
}
