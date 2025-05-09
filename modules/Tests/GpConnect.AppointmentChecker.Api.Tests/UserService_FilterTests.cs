using System.Text.Json;
using Dapper;
using gpconnect_appointment_checker.api.Service;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;
using SortDirection = GpConnect.AppointmentChecker.Api.Helpers.Enumerations.SortDirection;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class UserService_FilterTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    private IDatabase _redisDb;
    private RedisCacheService _cacheService;
    private INotificationService _notificationService;
    private IDataService _dataService;
    private UserService _userService;

    private const string PENDING_USERS_HASH_KEY = "users:pending";
    private const string PENDING_USERS_INDEX_HASH_KEY = "users:pending:index";
    private const string ALL_USERS_HASH_KEY = "users:all";
    private const string ALL_USERS_INDEX_HASH_KEY = "users:all:index";


    public UserService_FilterTests()
    {
        _redisContainer = new RedisBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        var muxer = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _redisDb = muxer.GetDatabase();
        _cacheService = new RedisCacheService(muxer, new LoggerFactory().CreateLogger<RedisCacheService>());

        _notificationService = Substitute.For<INotificationService>();
        _dataService = Substitute.For<IDataService>();
        var notificationConfig = new NotificationConfig
        {
            ApptCheckerApiKey = "test-api-key",
            AccountDeactivatedTemplateId = "deactivated-template-id",
            NewAccountCreatedTemplateId = "new-account-template-id",
            UserDetailsFormTemplateId = "details-template-id"
        };

        var notificationOption = Substitute.For<IOptions<NotificationConfig>>();
        notificationOption.Value.Returns(notificationConfig);

        var generalConfig = new GeneralConfig
        {
            ProductName = "TestProduct",
            ProductVersion = "1.0.0",
            MaxNumWeeksSearch = 12,
            MaxNumberProviderCodesSearch = 5,
            MaxNumberConsumerCodesSearch = 5,
            LogRetentionDays = 30,
            GetAccessEmailAddress = "test@example.com"
        };

        var generalOptionsMock = Substitute.For<IOptions<GeneralConfig>>();
        generalOptionsMock.Value.Returns(generalConfig);

        _userService = new UserService(_dataService, _notificationService, notificationOption, generalOptionsMock,
            _cacheService,
            new FakeLogger<UserService>());
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
    }

    [Fact]
    public async Task UserService_GetByStatus_ShouldReturnAllUsers_WhenInCache()
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = SortBy.EmailAddress,
            SortDirection = SortDirection.ASC,
            RequestUserId = 1,
            Surname = null,
            EmailAddress = null,
            UserAccountStatusFilter = UserAccountStatus.Pending,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null
        };

        var user1 = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var user2 = new User
        {
            UserId = 2,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test2@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User2"
        };

        var userJson = JsonSerializer.Serialize(new List<User> { user1, user2 });

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("page1", userJson),
            new HashEntry("totalItems", 2)
        ]);

        await _redisDb.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY, [
            new HashEntry(user1.UserId.ToString(), 1),
            new HashEntry(user2.UserId.ToString(), 1)
        ]);


        // Also populate all-users cache to pass other validations
        await _redisDb.HashSetAsync(ALL_USERS_HASH_KEY, [
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(ALL_USERS_INDEX_HASH_KEY, [
            new HashEntry(user1.UserId.ToString(), 1),
            new HashEntry(user2.UserId.ToString(), 1)
        ]);

        // Act
        var result = await _userService.GetUsersByStatus(userFilter.UserAccountStatusFilter);


        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Length.ShouldBe(2); // correct number of items (all)
        result.TotalItems.ShouldBe(2); // correct count returned from cache
        result.Items[0].UserId.ShouldBe(user1.UserId); //assert ordered
    }

    [Fact]
    public async Task UserService_GetByStatus_ShouldReturnAllUsersFromDb_WhenNotInCache()
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = SortBy.EmailAddress,
            SortDirection = SortDirection.ASC,
            RequestUserId = 1,
            Surname = null,
            EmailAddress = null,
            UserAccountStatusFilter = UserAccountStatus.Pending,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null
        };

        var user1 = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var user2 = new User
        {
            UserId = 2,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test2@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User2"
        };

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("totalItems", 0)
        ]);

        _dataService.ExecuteQuery<User>("application.get_users", Arg.Any<DynamicParameters?>())
            .Returns(Task.FromResult(new List<User> { user1, user2 }));

        var result = await _userService.GetUsersByStatus(userFilter.UserAccountStatusFilter);

        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Length.ShouldBe(2);
        result.TotalItems.ShouldBe(2);
        result.Items[0].UserId.ShouldBe(user1.UserId);
    }

    [Fact]
    public async Task UserService_GetUsersByFilter_ShouldReturnAllUsersFromDb_WhenNotInCache()
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = SortBy.EmailAddress,
            SortDirection = SortDirection.ASC,
            RequestUserId = 1,
            Surname = null,
            EmailAddress = null,
            UserAccountStatusFilter = UserAccountStatus.Pending,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null
        };

        var user1 = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var user2 = new User
        {
            UserId = 2,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test2@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User2"
        };

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("totalItems", 0)
        ]);

        _dataService.ExecuteQuery<User>("application.get_users", Arg.Any<DynamicParameters?>())
            .Returns(Task.FromResult(new List<User> { user1, user2 }));

        var result = await _userService.GetUsersByFilter(userFilter);

        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Length.ShouldBe(2);
        result.TotalItems.ShouldBe(2);
        result.Items[0].UserId.ShouldBe(user1.UserId);
    }


    [Fact]
    public async Task UserService_GetUsersByFilter_ShouldReturnAllUsersCache_WhenCacheAvailable()
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = SortBy.EmailAddress,
            SortDirection = SortDirection.ASC,
            RequestUserId = 1,
            Surname = null,
            EmailAddress = null,
            UserAccountStatusFilter = UserAccountStatus.Pending,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null
        };

        var user1 = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var user2 = new User
        {
            UserId = 2,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test2@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User2"
        };

        var userJson = JsonSerializer.Serialize(new[]
        {
            user1, user2
        });

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("totalItems", 2),
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY, [
            new HashEntry(user1.UserId.ToString(), 1),
            new HashEntry(user2.UserId.ToString(), 1)
        ]);


        // Also populate all-users cache to pass other validations
        await _redisDb.HashSetAsync(ALL_USERS_HASH_KEY, [
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(ALL_USERS_INDEX_HASH_KEY, [
            new HashEntry(user1.UserId.ToString(), 1),
            new HashEntry(user2.UserId.ToString(), 1)
        ]);


        var result = await _userService.GetUsersByFilter(userFilter);

        // assert uses cache - not db
        _dataService.DidNotReceive()
            .ExecuteQuery<User>(Arg.Any<string>(), Arg.Any<DynamicParameters?>());


        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Length.ShouldBe(2);
        result.TotalItems.ShouldBe(2);
        result.Items[0].UserId.ShouldBe(user1.UserId);
    }

    [Theory]
    [InlineData(SortBy.EmailAddress, SortDirection.ASC, UserAccountStatus.Pending, new[] { 1 })]
    [InlineData(SortBy.EmailAddress, SortDirection.ASC, UserAccountStatus.Authorised, new[] { 2 })]
    [InlineData(SortBy.EmailAddress, SortDirection.ASC, UserAccountStatus.Deauthorised, new[] { 3 })]
    public async Task UserService_GetByStatus_ShouldReturnUsersFromDb_WhenNotInCache(
        SortBy sortBy,
        SortDirection sortDirection,
        UserAccountStatus accountStatusFilter,
        int[] expectedUserIds)
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = sortBy,
            SortDirection = sortDirection,
            RequestUserId = 1,
            Surname = string.Empty,
            EmailAddress = string.Empty,
            UserAccountStatusFilter = accountStatusFilter,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null,
        };

        var pendingUser = new User
        {
            UserId = 1, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Pending, UserSessionId = 0,
            EmailAddress = "test@example.com", IsAdmin = true, MultiSearchEnabled = false, OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var authUser = new User
        {
            UserId = 2, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Authorised, UserSessionId = 0,
            EmailAddress = "test2@example.com", IsAdmin = true, MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false, DisplayName = "Admin User2"
        };

        var deAuthUser = new User
        {
            UserId = 3, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Deauthorised,
            UserSessionId = 0,
            EmailAddress = "test3@example.com", IsAdmin = true, MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false, DisplayName = "Admin User3"
        };


        _dataService.ExecuteQuery<User>("application.get_users", Arg.Any<DynamicParameters?>())
            .Returns(Task.FromResult(new List<User> { pendingUser, authUser, deAuthUser }));

        // Act
        var result = await _userService.GetUsersByStatus(userFilter.UserAccountStatusFilter);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Length.ShouldBe(expectedUserIds.Length);

        // Assert the UserIds in the expected order
        result.Items.Select(u => u.UserId).First().ShouldBe(expectedUserIds[0]);
    }

    [Theory]
    [InlineData(SortBy.EmailAddress, SortDirection.ASC, "meezajajabinks", new int[0])] // data we know won't be there
    [InlineData(SortBy.EmailAddress, SortDirection.ASC, "test@example.com", new[] { 1 })] // OK
    public async Task UserService_GetByFilter_ShouldReturnUsersFromDb_WhenNotInCache(
        SortBy sortBy,
        SortDirection sortDirection,
        string emailAddress,
        int[] expectedUserIds)
    {
        // Arrange
        var userFilter = new UserListAdvanced
        {
            SortByColumn = sortBy,
            SortDirection = sortDirection,
            RequestUserId = 1,
            Surname = string.Empty,
            EmailAddress = emailAddress,
            UserAccountStatusFilter = null,
            AccessLevelFilter = null,
            MultiSearchFilter = null,
            OrgTypeSearchFilter = null,
        };

        var pendingUser = new User
        {
            UserId = 1, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Pending, UserSessionId = 0,
            EmailAddress = "test@example.com", IsAdmin = true, MultiSearchEnabled = false, OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var authUser = new User
        {
            UserId = 2, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Authorised, UserSessionId = 0,
            EmailAddress = "test2@example.com", IsAdmin = true, MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false, DisplayName = "Admin User2"
        };

        var deAuthUser = new User
        {
            UserId = 3, OrganisationId = 0, UserAccountStatusId = (int)UserAccountStatus.Deauthorised,
            UserSessionId = 0,
            EmailAddress = "test3@example.com", IsAdmin = true, MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false, DisplayName = "Admin User3"
        };


        _dataService.ExecuteQuery<User>("application.get_users", Arg.Any<DynamicParameters?>())
            .Returns(Task.FromResult(new List<User> { pendingUser, authUser, deAuthUser }));

        // Act
        var result = await _userService.GetUsersByFilter(userFilter);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Length.ShouldBe(expectedUserIds.Length);

        // Assert the UserIds in the expected order
        if (expectedUserIds.Length > 0)
        {
            result.Items.Select(u => u.UserId).First().ShouldBe(expectedUserIds[0]);
        }
    }
}