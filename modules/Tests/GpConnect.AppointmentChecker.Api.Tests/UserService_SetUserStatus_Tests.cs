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
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;
using Shouldly;
using User = GpConnect.AppointmentChecker.Api.DTO.Response.Application.User;

namespace GpConnect.AppointmentChecker.Api.Tests;

public class UserServiceSetUserStatusTests
{
    private IConnectionMultiplexer _redis;
    private RedisCacheService _cacheService;
    private INotificationService _notificationService;
    private IDataService _dataService;
    private UserService _userService;
    private readonly IDatabase _mockDatabase;

    private const string PENDING_USERS_HASH_KEY = "users:pending";
    private const string PENDING_USERS_INDEX_HASH_KEY = "users:pending:index";
    private const string ALL_USERS_HASH_KEY = "users:all";
    private const string ALL_USERS_INDEX_HASH_KEY = "users:all:index";

    public UserServiceSetUserStatusTests()
    {
        _redis = Substitute.For<IConnectionMultiplexer>();
        _mockDatabase = Substitute.For<IDatabase>();
        _redis.GetDatabase().Returns(_mockDatabase);
        _cacheService = new RedisCacheService(_redis, new LoggerFactory().CreateLogger<RedisCacheService>());

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

    [Fact]
    public async Task UserService_SetUserStatus_ShouldCall_SetMultiSearch_WithTrue_When_Status_Is_Authorised()
    {
        // Arrange
        User user = new()
        {
            UserId = 1, MultiSearchEnabled = true, DisplayName = "Test User", EmailAddress = "test@example.com"
        };

        var statusUpdate = new UserUpdateStatus()
        {
            RequestUrl = "testurl.com",
            UserId = user.UserId,
            UserAccountStatusId = (int)UserAccountStatus.Authorised
        };

        _mockDatabase.HashGetAsync(ALL_USERS_HASH_KEY, "page1").Returns(JsonSerializer.Serialize(new[] { user }));
        _mockDatabase.HashGetAsync(PENDING_USERS_HASH_KEY, "page1").Returns(JsonSerializer.Serialize(new[] { user }));
        _mockDatabase.HashGetAsync(PENDING_USERS_INDEX_HASH_KEY, 1).Returns(1);
        _mockDatabase.HashGetAsync(ALL_USERS_INDEX_HASH_KEY, 1).Returns(1);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_user_status", Arg.Any<DynamicParameters>())
            .Returns(user);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_multi_search", Arg.Any<DynamicParameters>())
            .Returns(user);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.get_user_by_id", Arg.Any<DynamicParameters>())
            .Returns(user);


        // Act
        var result = await _userService.SetUserStatus(statusUpdate);

        // Assert
        result.ShouldNotBeNull();
        await _dataService.Received(1)
            .ExecuteQueryFirstOrDefault<User>(
                "application.set_multi_search",
                Arg.Is<DynamicParameters>(p =>
                    p.Get<int>("_user_id") == 1 &&
                    p.Get<bool>("_multi_search_enabled") == true));
    }


    [Theory]
    [InlineData(UserAccountStatus.Deauthorised)]
    [InlineData(UserAccountStatus.Pending)]
    [InlineData(UserAccountStatus.Unknown)]
    [InlineData(UserAccountStatus.RequestDenied)]
    public async Task UserService_SetUserStatus_ShouldNotCall_SetMultiSearch_When_Status_Is_NotAuthorised(
        UserAccountStatus status)
    {
        // Arrange

        User user = new()
        {
            UserId = 1,
            MultiSearchEnabled = true,
            DisplayName = "Test User",
            EmailAddress = "test@example.com"
        };

        _mockDatabase.HashGetAsync(ALL_USERS_HASH_KEY, "page1").Returns(JsonSerializer.Serialize(new[]
            {
                user
            }
        ));

        _mockDatabase.HashGetAsync(PENDING_USERS_HASH_KEY, "page1").Returns(JsonSerializer.Serialize(new[]
            {
                user
            }
        ));

        _mockDatabase.HashGetAsync(PENDING_USERS_INDEX_HASH_KEY, 1).Returns(1);
        _mockDatabase.HashGetAsync(ALL_USERS_INDEX_HASH_KEY, 1).Returns(1);

        var statusUpdate = new UserUpdateStatus()
        {
            RequestUrl = "testurl.com",
            UserId = 1,
            UserAccountStatusId = (int)status
        };

        _dataService.ExecuteQueryFirstOrDefault<User>("application.set_user_status", Arg.Any<DynamicParameters>())
            .Returns(user);

        _dataService.ExecuteQueryFirstOrDefault<User>("application.get_user_by_id", Arg.Any<DynamicParameters>())
            .Returns(user);


        // Act
        var result = await _userService.SetUserStatus(statusUpdate);

        // Assert
        result.ShouldNotBeNull();
        await _dataService.DidNotReceive()
            .ExecuteQueryFirstOrDefault<User>(
                "application.set_multi_search",
                Arg.Any<DynamicParameters>());
    }
}