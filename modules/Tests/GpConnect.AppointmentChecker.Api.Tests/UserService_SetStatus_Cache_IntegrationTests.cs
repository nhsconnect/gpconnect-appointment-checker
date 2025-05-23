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
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;
using Shouldly;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class UserServiceSetUserStatusCacheIntegrationTests : IAsyncLifetime
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

    public UserServiceSetUserStatusCacheIntegrationTests()
    {
        _redisContainer = new RedisBuilder().Build();
    }

    [Theory]
    [InlineData(UserAccountStatus.Unknown, UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.Unknown, UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Unknown, UserAccountStatus.Authorised)]
    [InlineData(UserAccountStatus.Unknown, UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Unknown, UserAccountStatus.RequestDenied)]
    [InlineData(UserAccountStatus.Pending, UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.Pending, UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Pending, UserAccountStatus.Authorised)]
    [InlineData(UserAccountStatus.Pending, UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Pending, UserAccountStatus.RequestDenied)]
    [InlineData(UserAccountStatus.Authorised, UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.Authorised, UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Authorised, UserAccountStatus.Authorised)]
    [InlineData(UserAccountStatus.Authorised, UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Authorised, UserAccountStatus.RequestDenied)]
    [InlineData(UserAccountStatus.Deauthorised, UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.Deauthorised, UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Deauthorised, UserAccountStatus.Authorised)]
    [InlineData(UserAccountStatus.Deauthorised, UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Deauthorised, UserAccountStatus.RequestDenied)]
    [InlineData(UserAccountStatus.RequestDenied, UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.RequestDenied, UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.RequestDenied, UserAccountStatus.Authorised)]
    [InlineData(UserAccountStatus.RequestDenied, UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.RequestDenied, UserAccountStatus.RequestDenied)]
    public async Task UserService_Updates_CachedUser_AfterSetStatus(
        UserAccountStatus startingStatus,
        UserAccountStatus newStatus)
    {
        // Arrange
        User afterUpdateUser = new()
        {
            UserId = 1,
            MultiSearchEnabled = true,
            DisplayName = "Test User",
            EmailAddress = "test@example.com",
            UserAccountStatusId = (int)newStatus,
        };

        User beforeUser = new()
        {
            UserId = 1,
            MultiSearchEnabled = true,
            DisplayName = "Test User",
            EmailAddress = "test@example.com",
            UserAccountStatusId = (int)startingStatus,
        };

        await PopulateCacheWithTestData([beforeUser]);

        var statusUpdate = new UserUpdateStatus()
        {
            RequestUrl = "testurl.com",
            UserId = beforeUser.UserId,
            UserAccountStatusId = (int)UserAccountStatus.Authorised
        };

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_user_status", Arg.Any<DynamicParameters>())
            .Returns(afterUpdateUser);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_multi_search", Arg.Any<DynamicParameters>())
            .Returns(afterUpdateUser);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.get_user_by_id", Arg.Any<DynamicParameters>())
            .Returns(beforeUser);


        // Act
        var result = await _userService.SetUserStatus(statusUpdate);

        // Assert
        result.ShouldNotBeNull();
        await _dataService.Received()
            .ExecuteQueryFirstOrDefault<User>(
                "application.set_multi_search",
                Arg.Is<DynamicParameters>(p =>
                    p.Get<int>("_user_id") == 1 &&
                    p.Get<bool>("_multi_search_enabled") == true));

        result.EmailAddress.ShouldBe(afterUpdateUser.EmailAddress);

        // assert cache is updated correctly with database user
        result.UserAccountStatusId.ShouldBe((int)newStatus);
    }


    [Theory]
    [InlineData(UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.RequestDenied)]
    public async Task UserServiceCache_Should_Not_ReturnMultiSearchEnabled_When_SetUserStatusWasCalled_WithNotAuthorise(
        UserAccountStatus status)
    {
        // Arrange

        User dbUser = new()
        {
            UserId = 1,
            MultiSearchEnabled = false,
            UserAccountStatusId = (int)status,
            DisplayName = "Test User",
            EmailAddress = "test@example.com"
        };


        var statusUpdate = new UserUpdateStatus()
        {
            RequestUrl = "testurl.com",
            UserId = 1,
            UserAccountStatusId = (int)status
        };

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_user_status", Arg.Any<DynamicParameters>())
            .Returns(dbUser);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.get_user_by_id", Arg.Any<DynamicParameters>())
            .Returns(dbUser);


        // Act
        var result = await _userService.SetUserStatus(statusUpdate);

        // Assert
        result.ShouldNotBeNull();
        await _dataService.DidNotReceive()
            .ExecuteQueryFirstOrDefault<User>(
                "application.set_multi_search",
                Arg.Is<DynamicParameters>(p =>
                    p.Get<int>("_user_id") == 1 &&
                    p.Get<bool>("_multi_search_enabled") == false));
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
        await _redisContainer.StopAsync();
    }

    private async Task PopulateCacheWithTestData(User[] users)
    {
        var userJson = JsonSerializer.Serialize(users);

        await _redisDb.HashSetAsync(PENDING_USERS_HASH_KEY, [
            new HashEntry("page1", userJson),
            new HashEntry("totalItems", users.Length)
        ]);

        await _redisDb.HashSetAsync(PENDING_USERS_INDEX_HASH_KEY,
            users.Select(u => new HashEntry(u.UserId.ToString(), 1)).ToArray());


        // Also populate all-users cache to pass other validations
        await _redisDb.HashSetAsync(ALL_USERS_HASH_KEY, [
            new HashEntry("page1", userJson)
        ]);

        await _redisDb.HashSetAsync(ALL_USERS_INDEX_HASH_KEY,
            users.Select(u => new HashEntry(u.UserId.ToString(), 1)).ToArray());
    }
}