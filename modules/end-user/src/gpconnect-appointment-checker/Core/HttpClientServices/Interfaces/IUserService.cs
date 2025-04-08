using System.Threading.Tasks;
using gpconnect_appointment_checker.Models;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;

namespace gpconnect_appointment_checker.Core.HttpClientServices.Interfaces;

public interface IUserService
{
    Task<Organisation> GetOrganisationAsync(string odsCode);
    Task<PagedData<User>> GetUsersAsync(UserListAdvanced userListAdvanced, int page);
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