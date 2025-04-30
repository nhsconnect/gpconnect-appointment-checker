using gpconnect_appointment_checker.api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace gpconnect_appointment_checker.api;

public class UserQueryHandler
{
    private readonly IUserService _userService;
    private readonly ICacheService _cacheService;

    public UserQueryHandler(IUserService userService, ICacheService cacheService)
    {
        _cacheService = cacheService;
        _userService = userService;
    }


    public async Task<PagedData<User>> HandleGetUserRequest(UserListAdvanced userFilter, int page)
    {
        if (IsDefaultFilter(userFilter))
        {
            return await _userService.GetAllUsers(page);
        }

        if (IsStatusOnly(userFilter))
        {
            return await _userService.GetUsersByStatus(userFilter.UserAccountStatusFilter, page);
        }

        return await _userService.GetUsersByFilter(userFilter, page);
    }

    private static bool IsDefaultFilter(UserListAdvanced filter) =>
        string.IsNullOrWhiteSpace(filter.EmailAddress) &&
        filter.AccessLevelFilter is null &&
        filter.MultiSearchFilter is null &&
        filter.OrgTypeSearchFilter is null &&
        string.IsNullOrWhiteSpace(filter.Surname) &&
        filter.UserAccountStatusFilter is null;

    private static bool IsStatusOnly(UserListAdvanced filter) =>
        filter.UserAccountStatusFilter is not null
        && string.IsNullOrWhiteSpace(filter.EmailAddress)
        && filter.AccessLevelFilter is null
        && filter.MultiSearchFilter is null
        && filter.OrgTypeSearchFilter is null
        && string.IsNullOrWhiteSpace(filter.Surname);
}
