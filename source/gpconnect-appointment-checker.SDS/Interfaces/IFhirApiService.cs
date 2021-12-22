using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers.Enumerations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface IFhirApiService
    {
        Task<List<OrganisationList>> GetOrganisationDetailsByOdsCode(List<string> odsCode, ErrorCode errorCodeToRaise);
        Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode);

        Task<List<SpineList>> GetProviderDetails(List<string> odsCode, ErrorCode errorCodeToRaise);        
        Task<Spine> GetProviderDetails(string odsCode);

        Task<List<SpineList>> GetConsumerDetails(List<string> odsCode, ErrorCode errorCodeToRaise);
        Task<Spine> GetConsumerDetails(string odsCode);
    }
}
