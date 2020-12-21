namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IApplicationService
    {
        DTO.Response.Application.Organisation GetOrganisation(string odsCode);
        void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation);
        DTO.Response.Application.User LogonUser(DTO.Request.Application.User user);
        DTO.Response.Application.User LogoffUser(DTO.Request.Application.User user);
        void SetUserAuthorised(DTO.Request.Application.User user);
    }
}
