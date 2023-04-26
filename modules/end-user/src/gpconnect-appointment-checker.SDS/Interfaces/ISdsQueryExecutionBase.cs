//using gpconnect_appointment_checker.DTO.Response.Application;
//using gpconnect_appointment_checker.DTO.Response.Configuration;
//using gpconnect_appointment_checker.Helpers.Enumerations;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace gpconnect_appointment_checker.SDS.Interfaces
//{
//    public interface ISdsQueryExecutionBase
//    {
//        Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode);
//        Task<List<OrganisationList>> GetOrganisationDetailsByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise);

//        Task<Spine> GetProviderDetails(string odsCode);
//        Task<List<SpineList>> GetProviderDetails(List<string> odsCodes, ErrorCode errorCodeToRaise);

//        Task<Spine> GetConsumerDetails(string odsCode);
//        Task<List<SpineList>> GetConsumerDetails(List<string> odsCodes, ErrorCode errorCodeToRaise);
//    }
//}
