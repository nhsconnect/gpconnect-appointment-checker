using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IApplicationService
    {
        Task<DTO.Response.Application.Organisation> GetOrganisation(string odsCode);
        void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation);
        void LogonUser(DTO.Request.Application.User user);
        void LogoffUser(DTO.Request.Application.User user);
        void SetUserAuthorised(DTO.Request.Application.User user);
    }
}
