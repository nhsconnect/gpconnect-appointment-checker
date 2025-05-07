using System.Text.Json;
using Dapper;
using gpconnect_appointment_checker.api.Service;
using gpconnect_appointment_checker.api.Service.Interfaces;
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
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class SetUserStatusTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    private ICacheService _cacheService;
    private INotificationService _notificationService;
    private IUserService _userService;
    private IDatabase _redisDb;
    private IDataService _dataService;

    private const string PENDING_USERS_HASH_KEY = "users:pending";
    private const string PENDING_USERS_INDEX_HASH_KEY = "users:pending:index";
    private const string ALL_USERS_HASH_KEY = "users:all";
    private const string ALL_USERS_INDEX_HASH_KEY = "users:all:index";

    public SetUserStatusTests()
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
    public async Task Should_RemoveUserFromPendingCache_WhenStatusMovesFromPending_And_DeletePageIfOnlyUserOnPage()
    {
        // Arrange
        var userBeingUpdated = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Authorised,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var userBeingUpdatedOriginalValues = new User()
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

        var userJson = JsonSerializer.Serialize(new List<User> { userBeingUpdatedOriginalValues });

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, new[]
        {
            new HashEntry("page1", userJson)
        });

        await _redisDb.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY, new[]
        {
            new HashEntry(userBeingUpdatedOriginalValues.UserId.ToString(), 1)
        });

        await _redisDb.HashSetAsync(ALL_USERS_HASH_KEY, new[]
        {
            new HashEntry("page1", userJson)
        });

        await _redisDb.HashSetAsync(ALL_USERS_INDEX_HASH_KEY, new[]
        {
            new HashEntry(userBeingUpdatedOriginalValues.UserId.ToString(), 1)
        });

        var updateRequest = new UserUpdateStatus
        {
            UserId = userBeingUpdated.UserId,
            UserAccountStatusId = (int)UserAccountStatus.Authorised,
            RequestUrl = "http://localhost"
        };

        // User is returned upon updating the db with new status.
        _dataService
            .ExecuteQueryFirstOrDefault<User>(Arg.Any<string>(), Arg.Any<DynamicParameters>())
            .Returns(Task.FromResult(userBeingUpdated));

        // Act
        await _userService.SetUserStatus(updateRequest);

        // Assert
        var remainingPage = await _cacheService.GetPageAsync<List<User>>(PENDING_USERS_HASH_KEY, "page1");
        remainingPage.ShouldBeNull();

        var indexEntry = await _redisDb.HashGetAsync(PENDING_USERS_INDEX_HASH_KEY, userBeingUpdated.UserId.ToString());
        indexEntry.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task
        Should_RemoveUserFromCache_WhenStatusMovesFromPending_But_NotDeletePage_WhenMoreThanOneUserOnPage()
    {
        // Arrange
        // Arrange
        var userBeingUpdated = new User
        {
            UserId = 1,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Authorised,
            UserSessionId = 0,
            EmailAddress = "test@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User"
        };

        var userBeingUpdatedOriginalValues = new User()
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


        var secondUser = new User()
        {
            UserId = 2,
            OrganisationId = 0,
            UserAccountStatusId = (int)UserAccountStatus.Pending,
            UserSessionId = 0,
            EmailAddress = "test2@example.com",
            IsAdmin = true,
            MultiSearchEnabled = false,
            OrgTypeSearchEnabled = false,
            DisplayName = "Admin User 2"
        };

        var userJson = JsonSerializer.Serialize(new List<User> { userBeingUpdatedOriginalValues, secondUser });

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY, [
            new HashEntry(userBeingUpdatedOriginalValues.UserId.ToString(), 1),
            new HashEntry(secondUser.UserId.ToString(), 2)
        ]);

        // Also populate all-users cache to pass other validations
        await _redisDb.HashSetAsync(ALL_USERS_HASH_KEY, [
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(ALL_USERS_INDEX_HASH_KEY, [
            new HashEntry(userBeingUpdatedOriginalValues.UserId.ToString(), 1),
            new HashEntry(secondUser.UserId.ToString(), 2)
        ]);

        var updateRequest = new UserUpdateStatus
        {
            UserId = userBeingUpdatedOriginalValues.UserId,
            UserAccountStatusId = (int)UserAccountStatus.Authorised,
            RequestUrl = "http://localhost"
        };

        _dataService
            .ExecuteQueryFirstOrDefault<User>("application.set_user_status", Arg.Any<DynamicParameters>())
            .Returns(Task.FromResult(userBeingUpdated));

        _dataService
            .ExecuteQuery<User>("application.get_users", Arg.Any<DynamicParameters>())
            .Returns(Task.FromResult(new List<User> { userBeingUpdated, secondUser }));

        _dataService.ExecuteQueryFirstOrDefault<User>("application.get_user_by_id", Arg.Any<DynamicParameters>())
            .Returns(Task.FromResult(userBeingUpdated));

        // Act
        await _userService.SetUserStatus(updateRequest);

        // Assertions

        // Assert page1 is still there after removing original user
        var remainingPage = await _cacheService.GetPageAsync<List<User>>(PENDING_USERS_HASH_KEY, "page1");
        remainingPage.ShouldNotBeNull();
        remainingPage.Find(x => x.UserId == secondUser.UserId).ShouldNotBeNull();
        remainingPage.Count.ShouldBe(1);
        remainingPage.SingleOrDefault(x => x.UserId == userBeingUpdatedOriginalValues.UserId).ShouldBeNull();

        // assert the user is removed from index pages
        var indexEntry = await _redisDb.HashGetAsync(
            PENDING_USERS_INDEX_HASH_KEY,
            userBeingUpdatedOriginalValues.UserId.ToString());

        indexEntry.HasValue.ShouldBeFalse();
    }
}