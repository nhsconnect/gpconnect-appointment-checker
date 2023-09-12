using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IUserService
{
    Task<Organisation> GetOrganisationAsync(string odsCode);
    Task<List<User>> GetUsersAsync(UserListSimple userListSimple);
    Task<List<User>> GetUsersAsync(UserListAdvanced userListAdvanced);
    Task<User> LogonUser(LogonUser user);
    Task<User> LogoffUser(LogoffUser user);
    Task SetUserStatus(UserUpdateStatus userUpdateStatus);
    Task SetMultiSearch(UserUpdateMultiSearch userUpdateMultiSearch);
    Task SetOrgTypeSearch(UserUpdateOrgTypeSearch userUpdateOrgTypeSearch);
    Task SetIsAdmin(UserUpdateIsAdmin userUpdateIsAdmin);
    Task<User> AddUserAsync(AddUser addUser);
    Task<User?> GetUser(string emailAddress);
    Task<User> AddOrUpdateUser(UserCreateAccount userCreateAccount);
}
