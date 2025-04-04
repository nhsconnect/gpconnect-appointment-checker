using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IUserService
{
    Task<Organisation> GetOrganisation(string odsCode);

    /// <summary>
    /// Gets all users from db first, applying given filter params & replenishes cache
    /// </summary>
    /// <param name="userListAdvanced"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    Task<PagedData<User>> GetUsersByFilter(UserListAdvanced userListAdvanced, int page = 1);

    /// <summary>
    /// Retrieves users by Status, cache first
    /// </summary>
    /// <param name="status"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    Task<PagedData<User>> GetUsersByStatus(UserAccountStatus? status, int page = 1);

    /// <summary>
    /// Gets all Users cache first
    /// </summary>
    /// <param name="page"></param>
    /// <returns>Array of Users</returns>
    Task<PagedData<User>> GetAllUsers(int page = 1);

    /// <summary>
    /// Repopulates base cache (pending, active, all users) with provided user list
    /// </summary>
    /// <param name="users">Users to be cached</param>
    /// <returns></returns>
    Task RebuildBaseUserCache(User[] users);

    Task<User> GetUserById(int userId);
    Task<User> LogonUser(DTO.Request.Application.LogonUser user);
    Task<User> LogoffUser(DTO.Request.Application.LogoffUser user);
    Task<User> AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount);
    Task<User> SetUserStatus(DTO.Request.Application.UserUpdateStatus userUpdateStatus);
    Task SetMultiSearch(DTO.Request.Application.UserUpdateMultiSearch userUpdateMultiSearch);
    Task SetIsAdmin(DTO.Request.Application.UserUpdateIsAdmin userUpdateIsAdmin);
    Task SetOrgTypeSearch(DTO.Request.Application.UserUpdateOrgTypeSearch userUpdateOrgTypeSearch);
    Task<User> AddUser(DTO.Request.Application.UserAdd userAdd);
    Task<User> GetUser(string emailAddress);
}