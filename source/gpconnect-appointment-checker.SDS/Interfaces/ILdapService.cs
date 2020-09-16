using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapService
    {
        Task<Dictionary<string, string>> GetOrganisationDetailsByOdsCode(string odsCode);
        Task<Dictionary<string, string>> OrganisationHasAppointmentsProviderSystemByOdsCode(string odsCode);
        Task<Dictionary<string, string>> OrganisationHasAppointmentsConsumerSystemByOdsCode(string odsCode);
        Task<Dictionary<string, string>> GetGpProviderEndpointAndAsIdByOdsCode(string odsCode);
    }
}
