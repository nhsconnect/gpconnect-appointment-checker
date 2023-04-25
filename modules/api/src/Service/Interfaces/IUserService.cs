using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsers(DTO.Request.Application.UserListSimple userListSimple);
    Task<IEnumerable<User>> GetUsers(DTO.Request.Application.UserListAdvanced userListAdvanced);
    Task<User> GetUserById(int userId);
    Task<User> LogonUser(DTO.Request.Application.LogonUser user);
    Task<User> LogoffUser(DTO.Request.Application.LogoffUser user);
    Task<User> AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount);
    Task<User> SetUserStatus(int userId, int userAccountStatusId, int adminUserId, int userSessionId);
    Task SetMultiSearch(int userId, bool multiSearchEnabled, int adminUserId, int userSessionId);
    Task SetOrgTypeSearch(int userId, bool orgTypeSearchEnabled, int adminUserId, int userSessionId);
    Task<User> AddUser(DTO.Request.Application.UserAdd userAdd);
    Task<User> GetUser(string emailAddress);
}
