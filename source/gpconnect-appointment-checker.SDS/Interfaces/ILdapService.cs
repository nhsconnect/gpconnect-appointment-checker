using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode);
        Task<Organisation> OrganisationHasAppointmentsProviderSystemByOdsCode(string odsCode);
        Task<Organisation> OrganisationHasAppointmentsConsumerSystemByOdsCode(string odsCode);
        Task<Spine> GetGpProviderEndpointAndAsIdByOdsCode(string odsCode);
    }
}
