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
    Task<User> LogonUser(LogonUser user);
    Task<User> LogoffUser(LogoffUser user);
    Task<User> AddOrUpdateUser(UserCreateAccount userCreateAccount);
    Task<User> SetUserStatus(UserUpdateStatus userUpdateStatus);
    Task SetMultiSearch(UserUpdateMultiSearch userUpdateMultiSearch, bool updateCache = true);
    Task SetIsAdmin(UserUpdateIsAdmin userUpdateIsAdmin);
    Task SetOrgTypeSearch(UserUpdateOrgTypeSearch userUpdateOrgTypeSearch);
    Task<User> AddUser(UserAdd userAdd);
    Task<User> GetUser(string emailAddress);
}