using GpConnect.AppointmentChecker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync(Models.Request.UserListSimple userListSimple);
    Task<List<User>> GetUsersAsync(Models.Request.UserListAdvanced userListAdvanced);
    Task<User> LogonUser(Models.Request.LogonUser user);
    Task<User> LogoffUser(Models.Request.LogoffUser user);
    Task<User> SetUserStatus(int userId, int userAccountStatusId);
    Task SetMultiSearch(int userId, bool multiSearchEnabled);
    Task SetOrgTypeSearch(int userId, bool orgTypeSearchEnabled);
    Task<User> AddUserAsync(string emailAddress);
    Task<User> GetUser(string emailAddress);
    Task<User> AddOrUpdateUser(Models.Request.UserCreateAccount userCreateAccount);
}
