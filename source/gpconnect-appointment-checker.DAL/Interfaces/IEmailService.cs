using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IEmailService
    {
        void SendUserStatusEmail(int userAccountStatusId, string recipient);
        void SendUserCreateAccountEmail(DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
