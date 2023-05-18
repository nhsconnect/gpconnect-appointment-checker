using Amazon.Runtime;
using Dapper;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Data;
using System.Linq.Dynamic.Core;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Service;

public class UserService : IUserService
{
    private readonly IDataService _dataService;
    private readonly INotificationService _notificationService;
    private readonly IOptions<NotificationConfig> _notificationConfig;
    private readonly IOptions<GeneralConfig> _generalConfig;

    public UserService(IDataService dataService, INotificationService notificationService, IOptions<NotificationConfig> notificationConfig, IOptions<GeneralConfig> generalConfig)
    {
        _dataService = dataService ?? throw new ArgumentNullException();
        _notificationService = notificationService ?? throw new ArgumentNullException();
        _notificationConfig = notificationConfig ?? throw new ArgumentNullException();
        _generalConfig = generalConfig ?? throw new ArgumentNullException();
    }

    public async Task<IEnumerable<User>> GetUsers(UserListSimple userListSimple)
    {
        var functionName = "application.get_users";
        var filteredList = (await _dataService.ExecuteQuery<User>(functionName)).AsQueryable();
        var orderedList = filteredList.OrderBy($"{userListSimple.SortByColumn} {userListSimple.SortDirection}");
        return orderedList;
    }

    public async Task<IEnumerable<User>> GetUsers(UserListAdvanced userListAdvanced)
    {
        var functionName = "application.get_users";
        var filteredList = (await _dataService.ExecuteQuery<User>(functionName)).AsQueryable();
        filteredList = ApplyFilters(userListAdvanced, filteredList);
        return filteredList.OrderBy($"{userListAdvanced.SortByColumn} {userListAdvanced.SortDirection}");
    }

    private static IQueryable<User> ApplyFilters(UserListAdvanced userListAdvanced, IQueryable<User> filteredList)
    {
        filteredList = !string.IsNullOrEmpty(userListAdvanced.Surname) ? filteredList.Where(x => x.DisplayName.Contains(userListAdvanced.Surname, StringComparison.OrdinalIgnoreCase)) : filteredList;
        filteredList = !string.IsNullOrEmpty(userListAdvanced.EmailAddress) ? filteredList.Where(x => x.EmailAddress.Contains(userListAdvanced.EmailAddress, StringComparison.OrdinalIgnoreCase)) : filteredList;
        filteredList = !string.IsNullOrEmpty(userListAdvanced.OrganisationName) ? filteredList.Where(x => x.OrganisationName.Contains(userListAdvanced.OrganisationName, StringComparison.OrdinalIgnoreCase)) : filteredList;

        if (userListAdvanced.UserAccountStatusFilter != null)
        {
            filteredList = filteredList.Where(x => x.UserAccountStatusId == (int)userListAdvanced.UserAccountStatusFilter);
        }
        if (userListAdvanced.AccessLevelFilter != null)
        {
            filteredList = filteredList.Where(x => Enum.Parse<AccessLevel>(x.AccessLevel) == userListAdvanced.AccessLevelFilter);
        }
        if (userListAdvanced.MultiSearchFilter != null)
        {
            filteredList = filteredList.Where(x => x.MultiSearchEnabled == userListAdvanced.MultiSearchFilter);
        }
        if (userListAdvanced.OrgTypeSearchFilter != null)
        {
            filteredList = filteredList.Where(x => x.OrgTypeSearchEnabled == userListAdvanced.OrgTypeSearchFilter);
        }
        return filteredList;
    }

    public async Task<User> LogonUser(LogonUser user)
    {
        var functionName = "application.logon_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", user.EmailAddress);
        parameters.Add("_display_name", user.DisplayName);
        parameters.Add("_organisation_id", user.OrganisationId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    public async Task<User> LogoffUser(LogoffUser user)
    {
        var functionName = "application.logoff_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", user.EmailAddress);
        parameters.Add("_user_session_id", user.UserSessionId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    public async Task<User> AddOrUpdateUser(UserCreateAccount userCreateAccount)
    {
        var functionName = "application.add_or_update_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", userCreateAccount.EmailAddress);
        parameters.Add("_display_name", userCreateAccount.DisplayName);
        parameters.Add("_organisation_id", userCreateAccount.OrganisationId);
        parameters.Add("_user_account_status_id", (int)userCreateAccount.UserAccountStatus);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await SendNewUserNotification(userCreateAccount);

        return result;
    }

    public async Task<User> SetUserStatus(UserUpdateStatus userUpdateStatus)
    {
        var functionName = "application.set_user_status";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id", userUpdateStatus.AdminUserId);
        parameters.Add("_user_id", userUpdateStatus.UserId);
        parameters.Add("_user_account_status_id", userUpdateStatus.UserAccountStatusId);
        parameters.Add("_user_session_id", userUpdateStatus.UserSessionId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await SendUserStatusNotification(userUpdateStatus);

        return result;
    }

    private async Task SendNewUserNotification(UserCreateAccount userCreateAccount)
    {
        var notificationRequest = new NotificationCreateRequest()
        {
            EmailAddresses = new List<string>() { _generalConfig.Value.GetAccessEmailAddress },
            TemplateId = _notificationConfig.Value.UserDetailsFormTemplateId,
            RequestUrl = userCreateAccount.RequestUrl
        };

        notificationRequest.TemplateParameters.Add("email_address", userCreateAccount.EmailAddress);
        notificationRequest.TemplateParameters.Add("job_role", userCreateAccount.JobRole);
        notificationRequest.TemplateParameters.Add("organisation_name", userCreateAccount.OrganisationName);
        notificationRequest.TemplateParameters.Add("access_reason", userCreateAccount.Reason);

        await _notificationService.PostNotificationAsync(notificationRequest);
    }

    private async Task SendUserStatusNotification(UserUpdateStatus userUpdateStatus)
    {
        var user = await GetUserById(userUpdateStatus.UserId);

        var notificationRequest = new NotificationCreateRequest()
        {
            EmailAddresses = new List<string>() { user.EmailAddress },
            RequestUrl = userUpdateStatus.RequestUrl
        };

        switch (userUpdateStatus.UserAccountStatusId)
        {
            case (int)UserAccountStatus.Deauthorised:
                notificationRequest.TemplateId = _notificationConfig.Value.AccountDeactivatedTemplateId;                
                break;
            case (int)UserAccountStatus.Authorised:
                notificationRequest.TemplateId = _notificationConfig.Value.NewAccountCreatedTemplateId;
                break;
        }
        await _notificationService.PostNotificationAsync(notificationRequest);
    }

    public async Task SetMultiSearch(UserUpdateMultiSearch userUpdateMultiSearch)
    {
        var functionName = "application.set_multi_search";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id", userUpdateMultiSearch.AdminUserId);
        parameters.Add("_user_id", userUpdateMultiSearch.UserId);
        parameters.Add("_multi_search_enabled", userUpdateMultiSearch.MultiSearchEnabled);
        parameters.Add("_user_session_id", userUpdateMultiSearch.UserSessionId);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task SetOrgTypeSearch(UserUpdateOrgTypeSearch userUpdateOrgTypeSearch)
    {
        var functionName = "application.set_org_type_search";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id", userUpdateOrgTypeSearch.AdminUserId);
        parameters.Add("_user_id", userUpdateOrgTypeSearch.UserId);
        parameters.Add("_org_type_search_enabled", userUpdateOrgTypeSearch.OrgTypeSearchEnabled);
        parameters.Add("_user_session_id", userUpdateOrgTypeSearch.UserSessionId);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<User> AddUser(UserAdd userAdd)
    {
        var functionName = "application.add_user_manual";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", userAdd.EmailAddress);
        parameters.Add("_admin_user_id", userAdd.AdminUserId);
        parameters.Add("_user_session_id", userAdd.UserSessionId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    public async Task<User> GetUser(string emailAddress)
    {
        var functionName = "application.get_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", emailAddress);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    public async Task<User> GetUserById(int userId)
    {
        var functionName = "application.get_user_by_id";
        var parameters = new DynamicParameters();
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }
}
