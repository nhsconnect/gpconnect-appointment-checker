using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IEmailService
{
    bool SendUserStatusEmail(int userId, int userAccountStatusId, string recipient, bool userStatusChanged = false);
    bool SendUserCreateAccountEmail(User createdUser, DTO.Request.Application.UserCreateAccount userCreateAccount);
}
