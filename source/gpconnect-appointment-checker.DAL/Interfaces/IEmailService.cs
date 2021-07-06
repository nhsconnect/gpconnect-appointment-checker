using gpconnect_appointment_checker.DTO.Response.Application;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IEmailService
    {
        bool SendUserStatusEmail(int userId, int userAccountStatusId, string recipient);
        bool SendUserCreateAccountEmail(User createdUser, DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
