using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IEmailService
    {
        bool SendUserStatusEmail(int userAccountStatusId, string recipient);
        bool SendUserCreateAccountEmail(DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
