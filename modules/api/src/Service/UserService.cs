using System.Data;
using System.Text.Json;
using Dapper;
using gpconnect_appointment_checker.api.Helpers;
using gpconnect_appointment_checker.api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace gpconnect_appointment_checker.api.Service;

public class UserService : IUserService
{
    private readonly IDataService _dataService;
    private readonly INotificationService _notificationService;
    private readonly IOptions<NotificationConfig> _notificationConfig;
    private readonly IOptions<GeneralConfig> _generalConfig;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserService> _logger;

    private const string ALL_USERS_HASH_KEY = "users:all";
    private const string PENDING_USERS_HASH_KEY = $"users:pending";
    private const string ALL_USERS_INDEX_HASH_KEY = "users:all:index";
    private const string PENDING_USERS_INDEX_HASH_KEY = "users:pending:index";

    public UserService(IDataService dataService, INotificationService notificationService,
        IOptions<NotificationConfig> notificationConfig, IOptions<GeneralConfig> generalConfig,
        ICacheService cacheService, ILogger<UserService> logger)
    {
        _dataService = dataService ?? throw new ArgumentNullException();
        _notificationService = notificationService ?? throw new ArgumentNullException();
        _notificationConfig = notificationConfig ?? throw new ArgumentNullException();
        _generalConfig = generalConfig ?? throw new ArgumentNullException();
        _cacheService = cacheService ?? throw new ArgumentNullException();
        _logger = logger ?? throw new ArgumentNullException();
    }

    public async Task<Organisation> GetOrganisation(string odsCode)
    {
        const string functionName = "application.get_organisation";
        var parameters = new DynamicParameters();
        parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
        var result = await _dataService.ExecuteQueryFirstOrDefault<Organisation>(functionName, parameters);
        return result;
    }

    public async Task<PagedData<User>> GetUsersByFilter(UserListAdvanced filter, int page = 1)
    {
        try
        {
            HashEntry[] hashEntries = [];
            var cacheKey = string.Empty;

            cacheKey = filter.UserAccountStatusFilter == UserAccountStatus.Pending
                ? PENDING_USERS_HASH_KEY
                : ALL_USERS_HASH_KEY;

            hashEntries = await _cacheService.GetAllHashFieldsForHashSetAsync(cacheKey);

            User[] allUsers = [];

            // if no pages in the cache - goto db
            if (!hashEntries.Any(x => x.Name.ToString().Contains("page")))
            {
                allUsers = (await RetrieveUsersAndRebuildCache()).ToArray();
            }
            else
            {
                allUsers = hashEntries
                    .Where(e => e.Name.ToString().StartsWith("page"))
                    .SelectMany(e => JsonSerializer.Deserialize<User[]>(e.Value.ToString()) ?? [])
                    .ToArray();
            }

            if (allUsers.Length == 0)
            {
                _logger.LogWarning("No users found");
                return new PagedData<User>([])
                {
                    TotalItems = 0,
                    PageSize = 50
                };
            }

            var filteredUsers = ApplyFilters(filter, allUsers.AsQueryable()).ToArray();
            var pagedUsers = PagingHelpers.Paginate(filteredUsers.ToArray());
            return new PagedData<User>(pagedUsers.ToArray().Length == 0 ? [] : pagedUsers[page - 1])
            {
                TotalItems = filteredUsers.Length,
                PageSize = 50
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }


    public async Task<PagedData<User>> GetUsersByStatus(UserAccountStatus? status, int page = 1)
    {
        if (status == null)
        {
            throw new ArgumentNullException(nameof(status));
        }

        try
        {
            if (status == UserAccountStatus.Pending)
            {
                var pageData =
                    await _cacheService.GetPageAsync<User[]>(PENDING_USERS_HASH_KEY, $"page{page}");

                if (pageData is not null)
                {
                    return new PagedData<User>(PagingHelpers.Paginate(pageData)[page - 1])
                    {
                        TotalItems = await _cacheService.GetPageAsync<int>(PENDING_USERS_HASH_KEY, "totalItems"),
                        PageSize = 50
                    };
                }
            }

            // no cache - get All users and filter by given status
            var dbUsers = await RetrieveUsersAndRebuildCache();
            var statusUsers = ApplyFilters(new UserListAdvanced()
            {
                UserAccountStatusFilter = status
            }, dbUsers.AsQueryable()).ToArray();

            return new PagedData<User>(statusUsers.Length > 0 ? PagingHelpers.Paginate(statusUsers)[page - 1] : [])
            {
                TotalItems = statusUsers.Length,
                PageSize = 50
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }


    public async Task<PagedData<User>> GetAllUsers(int page = 1)
    {
        try
        {
            var pageData = await _cacheService.GetPageAsync<User[]>(ALL_USERS_HASH_KEY, $"page{page}");
            if (pageData is not null)
            {
                var totalUsers = await _cacheService.GetPageAsync<int>(ALL_USERS_HASH_KEY, "totalItems");
                return new PagedData<User>(pageData)
                {
                    TotalItems = totalUsers,
                    PageSize = 50
                };
            }

            var dbUsers = (await RetrieveUsersAndRebuildCache()).ToArray();

            return new PagedData<User>(PagingHelpers.Paginate(dbUsers)[page - 1])
            {
                TotalItems = dbUsers.ToArray().Length,
                PageSize = 50,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task RebuildBaseUserCache(User[] users)
    {
        await _cacheService.RemoveAsync(ALL_USERS_HASH_KEY);
        await _cacheService.RemoveAsync(PENDING_USERS_HASH_KEY);

        await _cacheService.RemoveAsync(ALL_USERS_INDEX_HASH_KEY);
        await _cacheService.RemoveAsync(PENDING_USERS_INDEX_HASH_KEY);

        var userIndex = new List<HashEntry>();
        var pendingUserIndex = new List<HashEntry>();

        var pagedPendingUsers = PagingHelpers
            .Paginate(users.Where(x => x.UserAccountStatusId == (int)UserAccountStatus.Pending)
                .ToArray());

        var pagedAllUsers = PagingHelpers.Paginate(users);

        // Create a batch to queue all operations
        var batch = _cacheService.CreateBatch();

        var tasks = new List<Task>
        {
            batch.HashSetAsync(ALL_USERS_HASH_KEY, "totalItems", pagedAllUsers.Sum(x => x.Length)),
            batch.HashSetAsync(PENDING_USERS_HASH_KEY, "totalItems", pagedPendingUsers.Sum(x => x.Length))
        };

        for (var page = 0; page < pagedAllUsers.Length; page++)
        {
            var pageKey = $"page{page + 1}";
            var jsonData = JsonSerializer.Serialize(pagedAllUsers[page]);
            tasks.Add(batch.HashSetAsync(ALL_USERS_HASH_KEY, pageKey, jsonData));

            // Track user index
            userIndex.AddRange(
                pagedAllUsers[page].Select(user => new HashEntry(user.UserId.ToString(), pageKey)));
        }

        for (var page = 0; page < pagedPendingUsers.Length; page++)
        {
            var pageKey = $"page{page + 1}";
            var jsonData = JsonSerializer.Serialize(pagedPendingUsers[page]);
            tasks.Add(batch.HashSetAsync(PENDING_USERS_HASH_KEY, pageKey, jsonData));

            // Track user index
            pendingUserIndex.AddRange(
                pagedPendingUsers[page].Select(user => new HashEntry(user.UserId.ToString(), pageKey)));
        }

        // Store user index mappings in Redis
        if (userIndex.Count != 0)
        {
            tasks.Add(batch.HashSetAsync(ALL_USERS_INDEX_HASH_KEY, userIndex.ToArray()));
        }

        if (pendingUserIndex.Count != 0)
        {
            tasks.Add(batch.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY, pendingUserIndex.ToArray()));
        }


        batch.Execute();
        await Task.WhenAll(tasks);

        // Finally update TTL on HSets
        tasks.Add(batch.KeyExpireAsync(PENDING_USERS_HASH_KEY, TimeSpan.FromHours(24)));
        tasks.Add(batch.KeyExpireAsync(ALL_USERS_HASH_KEY, TimeSpan.FromHours(24)));
    }

    public async Task UpdateCacheRecord(User user)
    {
        try
        {
            // Get the user's page number from page index cache
            var allUsersPageKey = await _cacheService.HashGetAsync(ALL_USERS_INDEX_HASH_KEY, user.UserId.ToString());

            var pendingUsersPageKey = await _cacheService.HashGetAsync(
                PENDING_USERS_INDEX_HASH_KEY,
                user.UserId.ToString()
            );

            // ----- Update Main User Cache ----------
            if (allUsersPageKey == 0)
            {
                _logger.LogWarning($"User {user.UserId} not found in cache index.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            var pageUserData = await _cacheService.GetPageAsync<User[]>(ALL_USERS_HASH_KEY, $"page{allUsersPageKey}");
            if (pageUserData is null)
            {
                _logger.LogWarning($"Page {allUsersPageKey} not found in cache.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            var userIndex = Array.FindIndex(pageUserData, u => u.UserId == user.UserId);
            if (userIndex == -1)
            {
                _logger.LogWarning($"User {user.UserId} not found in cached page {allUsersPageKey}.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            pageUserData[userIndex] = user;
            var updatedJson = JsonSerializer.Serialize(pageUserData);
            await _cacheService.SetPageAsync(ALL_USERS_HASH_KEY, $"page{allUsersPageKey}", updatedJson);

            // ----- Update pending cache -------

            // if status is not pending - ensure it doesn't exist in the pending cache
            if (user.UserAccountStatusId != (int)UserAccountStatus.Pending)
            {
                await CheckAndRemovePendingUser(user);
                return;
            }

            if (pendingUsersPageKey == 0)
            {
                _logger.LogInformation($"User: {user.UserId} not found in pending user index cache.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            var pagePendingUserData =
                await _cacheService.GetPageAsync<User[]>(ALL_USERS_HASH_KEY, $"page{allUsersPageKey}");

            if (pagePendingUserData == null)
            {
                _logger.LogWarning($"Page{allUsersPageKey} not found in pending cache.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            var pendingUserIndex = Array.FindIndex(pageUserData, u => u.UserId == user.UserId);
            if (pendingUserIndex == -1)
            {
                _logger.LogWarning($"User {user.UserId} not found in cached page {pendingUsersPageKey}.");
                await RetrieveUsersAndRebuildCache();
                return;
            }

            pageUserData[pendingUserIndex] = user;
            var pendingJson = JsonSerializer.Serialize(pagePendingUserData);
            await _cacheService.SetPageAsync(PENDING_USERS_HASH_KEY, $"page{pendingUsersPageKey}", pendingJson);

            _logger.LogInformation($"User: {user.UserId} updated in cache (page: {pendingUsersPageKey}).");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating user in cache.");
            throw;
        }
    }


    private async Task<IEnumerable<User>> RetrieveUsersAndRebuildCache()
    {
        const string functionName = "application.get_users";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        var allUsers = await _dataService.ExecuteQuery<User>(functionName, parameters);

        // Re-populate the cache with latest data
        await RebuildBaseUserCache(allUsers.ToArray());
        return allUsers;
    }

    private static IQueryable<User> ApplyFilters(UserListAdvanced userListAdvanced, IQueryable<User> usersToFilter)
    {
        usersToFilter = !string.IsNullOrEmpty(userListAdvanced.Surname)
            ? usersToFilter.Where(x =>
                x.DisplayName.Contains(userListAdvanced.Surname, StringComparison.OrdinalIgnoreCase))
            : usersToFilter;
        usersToFilter = !string.IsNullOrEmpty(userListAdvanced.EmailAddress)
            ? usersToFilter.Where(x =>
                x.EmailAddress.Contains(userListAdvanced.EmailAddress, StringComparison.OrdinalIgnoreCase))
            : usersToFilter;

        if (userListAdvanced.UserAccountStatusFilter != null)
        {
            usersToFilter =
                usersToFilter.Where(x => x.UserAccountStatusId == (int)userListAdvanced.UserAccountStatusFilter);
        }

        if (userListAdvanced.AccessLevelFilter != null)
        {
            usersToFilter = usersToFilter.Where(x =>
                Enum.Parse<AccessLevel>(x.AccessLevel) == userListAdvanced.AccessLevelFilter);
        }

        if (userListAdvanced.MultiSearchFilter != null)
        {
            usersToFilter = usersToFilter.Where(x => x.MultiSearchEnabled == userListAdvanced.MultiSearchFilter);
        }

        if (userListAdvanced.OrgTypeSearchFilter != null)
        {
            usersToFilter = usersToFilter.Where(x => x.OrgTypeSearchEnabled == userListAdvanced.OrgTypeSearchFilter);
        }

        return usersToFilter;
    }

    public async Task<User> LogonUser(LogonUser user)
    {
        const string functionName = "application.logon_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", user.EmailAddress);
        parameters.Add("_display_name", user.DisplayName);
        parameters.Add("_organisation_id", user.OrganisationId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        // await UpdateCacheRecord(result);
        return result;
    }

    public async Task<User> LogoffUser(LogoffUser user)
    {
        const string functionName = "application.logoff_user";
        var parameters = new DynamicParameters();
        parameters.Add("_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await UpdateCacheRecord(result);
        return result;
    }

    public async Task<User> AddOrUpdateUser(UserCreateAccount userCreateAccount)
    {
        const string functionName = "application.add_or_update_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", userCreateAccount.EmailAddress);
        parameters.Add("_display_name", userCreateAccount.DisplayName);
        parameters.Add("_organisation_id", userCreateAccount.OrganisationId);
        parameters.Add("_user_account_status_id", (int)userCreateAccount.UserAccountStatus);

        var adminUserId =
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId);
        if (adminUserId > 0)
        {
            parameters.Add("_admin_user_id", adminUserId, DbType.Int32);
        }
        else
        {
            parameters.Add("_admin_user_id", DBNull.Value, DbType.Int32);
        }

        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await SendNewUserNotification(userCreateAccount);

        // replenish cache
        await UpdateCacheRecord(result);
        return result;
    }

    public async Task<User> SetUserStatus(UserUpdateStatus userUpdateStatus)
    {
        const string functionName = "application.set_user_status";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        parameters.Add("_user_id", userUpdateStatus.UserId);
        parameters.Add("_user_account_status_id", userUpdateStatus.UserAccountStatusId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        switch (userUpdateStatus.UserAccountStatusId)
        {
            case (int)UserAccountStatus.Deauthorised:
            case (int)UserAccountStatus.Authorised:
                var user = await GetUserById(userUpdateStatus.UserId);

                var notificationRequest = new NotificationCreateRequest()
                {
                    EmailAddresses = new List<string> { user.EmailAddress },
                    RequestUrl = userUpdateStatus.RequestUrl,
                    TemplateId = GetTemplateForUserUpdateStatus(userUpdateStatus.UserAccountStatusId)
                };
                await _notificationService.PostNotificationAsync(notificationRequest);
                break;
        }

        // replenish cache
        await UpdateCacheRecord(result);

        return result;
    }

    private string GetTemplateForUserUpdateStatus(int userAccountStatusId)
    {
        return userAccountStatusId switch
        {
            (int)UserAccountStatus.Deauthorised => _notificationConfig.Value.AccountDeactivatedTemplateId,
            (int)UserAccountStatus.Authorised => _notificationConfig.Value.NewAccountCreatedTemplateId,
            _ => string.Empty
        };
    }

    private async Task SendNewUserNotification(UserCreateAccount userCreateAccount)
    {
        var notificationRequest = new NotificationCreateRequest()
        {
            EmailAddresses = [_generalConfig.Value.GetAccessEmailAddress],
            TemplateId = _notificationConfig.Value.UserDetailsFormTemplateId,
            RequestUrl = userCreateAccount.RequestUrl
        };

        notificationRequest.TemplateParameters.Add("email_address", userCreateAccount.EmailAddress);
        notificationRequest.TemplateParameters.Add("job_role", userCreateAccount.JobRole);
        notificationRequest.TemplateParameters.Add("organisation_name", userCreateAccount.OrganisationName);
        notificationRequest.TemplateParameters.Add("access_reason", userCreateAccount.Reason);

        await _notificationService.PostNotificationAsync(notificationRequest);
    }

    public async Task SetIsAdmin(UserUpdateIsAdmin userUpdateIsAdmin)
    {
        const string functionName = "application.set_user_is_admin";
        var parameters = new DynamicParameters();
        var userId = LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId);

        parameters.Add("_admin_user_id", userId);
        parameters.Add("_user_id", userUpdateIsAdmin.UserId);
        parameters.Add("_is_admin", userUpdateIsAdmin.IsAdmin);
        var updatedUser = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await UpdateCacheRecord(updatedUser);
    }

    public async Task SetMultiSearch(UserUpdateMultiSearch userUpdateMultiSearch)
    {
        const string functionName = "application.set_multi_search";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        parameters.Add("_user_id", userUpdateMultiSearch.UserId);
        parameters.Add("_multi_search_enabled", userUpdateMultiSearch.MultiSearchEnabled);
        var updatedUser = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        // replenish cache
        await UpdateCacheRecord(updatedUser);
    }

    public async Task SetOrgTypeSearch(UserUpdateOrgTypeSearch userUpdateOrgTypeSearch)
    {
        const string functionName = "application.set_org_type_search";
        var parameters = new DynamicParameters();
        parameters.Add("_admin_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        parameters.Add("_user_id", userUpdateOrgTypeSearch.UserId);
        parameters.Add("_org_type_search_enabled", userUpdateOrgTypeSearch.OrgTypeSearchEnabled);
        var updatedUser = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        await UpdateCacheRecord(updatedUser);
    }

    public async Task<User> AddUser(UserAdd userAdd)
    {
        const string functionName = "application.add_user_manual";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", userAdd.EmailAddress);
        parameters.Add("_admin_user_id",
            LoggingHelper.GetIntegerValue(GpConnect.AppointmentChecker.Api.Helpers.Constants.Headers.UserId));
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);

        //TODO: find nicer way for adjusting cache after adding new user rather than re-building
        await RetrieveUsersAndRebuildCache();

        return result;
    }

    public async Task<User> GetUser(string emailAddress)
    {
        const string functionName = "application.get_user";
        var parameters = new DynamicParameters();
        parameters.Add("_email_address", emailAddress);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    public async Task<User> GetUserById(int userId)
    {
        const string functionName = "application.get_user_by_id";
        var parameters = new DynamicParameters();
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<User>(functionName, parameters);
        return result;
    }

    private async Task CheckAndRemovePendingUser(User user)
    {
        var pendingUsersPageKey =
            await _cacheService.HashGetAsync(PENDING_USERS_INDEX_HASH_KEY, user.UserId.ToString());

        if (!pendingUsersPageKey.HasValue)
        {
            _logger.LogInformation($"User {user.UserId} not found in pending users index.");
            return;
        }

        var pageKey = $"page{pendingUsersPageKey}";
        var pageUsers = await _cacheService.GetPageAsync<List<User>>(PENDING_USERS_HASH_KEY, pageKey);

        if (pageUsers == null)
        {
            _logger.LogWarning($"Pending user page {pageKey} not found.");
            return;
        }

        var originalCount = pageUsers.Count;
        pageUsers.RemoveAll(u => u.UserId == user.UserId);

        if (pageUsers.Count == originalCount)
        {
            _logger.LogInformation($"User {user.UserId} not found in pending user page {pageKey}.");
            return;
        }

        if (pageUsers.Count == 0)
        {
            _logger.LogInformation($"Removing empty page {pageKey} from pending users cache.");
            await _cacheService.HashDeleteAsync(PENDING_USERS_HASH_KEY, pageKey);
            await _cacheService.HashDeleteAsync(PENDING_USERS_INDEX_HASH_KEY, user.UserId.ToString());
        }
        else
        {
            _logger.LogInformation("Re-building cache");

            //TODO: rather than re-building cache shift all users up 1 page if > 1 page of pending users (unlikely) 
            await RetrieveUsersAndRebuildCache();
        }
    }
}