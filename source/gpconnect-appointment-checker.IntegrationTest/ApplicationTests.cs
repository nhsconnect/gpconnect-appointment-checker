using Dapper.FluentMap;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Application;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace gpconnect_appointment_checker.IntegrationTest
{

    [Collection("Sequential")]
    public class ApplicationTests
    {
        private readonly ApplicationService _applicationService;
        private readonly DataService _dataService;
        private readonly User _user;

        public ApplicationTests()
        {
            var mockLogger = new Mock<ILogger<ApplicationService>>();
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockLogService = new Mock<ILogService>();
            var mockAuditService = new Mock<IAuditService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContextAccessor = SetupContextAccessor();

            SetupFluentMappings();
            SetupConfiguration(mockConfiguration);

            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
            _applicationService = new ApplicationService(mockConfiguration.Object, mockLogger.Object, _dataService, mockAuditService.Object, mockLogService.Object, httpContextAccessor, mockEmailService.Object);

            _user = AddUserDetailsToContext(httpContextAccessor);
        }

        [Theory]
        [InlineData("A20047", "PR", "DR LEGG'S SURGERY", "LS1 4HY")]
        [InlineData("B82617", "PR", "COXWOLD SURGERY", "YO61 4BB")]
        public void OrganisationFound(string odsCode, string organisationTypeCode, string organisationName, string postalCode)
        {
            var result = _applicationService.GetOrganisation(odsCode);
            Assert.IsType<Organisation>(result);
            Assert.Equal(odsCode, result.ODSCode);
            Assert.Equal(organisationTypeCode, result.OrganisationTypeCode);
            Assert.Equal(organisationName, result.OrganisationName);
            Assert.Equal(postalCode, result.PostalCode);
            Assert.True(result.PostalAddressFields.Length > 0);
        }

        [Theory]
        [InlineData("X00000")]
        [InlineData("Y00000")]
        public void OrganisationNotFound(string odsCode)
        {
            var result = _applicationService.GetOrganisation(odsCode);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(SortBy.EmailAddress, SortDirection.ASC)]
        [InlineData(SortBy.AccessRequestCount, SortDirection.ASC)]
        [InlineData(SortBy.LastLogonDate, SortDirection.ASC)]
        [InlineData(SortBy.EmailAddress, SortDirection.DESC)]
        [InlineData(SortBy.AccessRequestCount, SortDirection.DESC)]
        [InlineData(SortBy.LastLogonDate, SortDirection.DESC)]
        public void UsersFound(SortBy sortBy, SortDirection sortDirection)
        {
            var result = _applicationService.GetUsers(sortBy, sortDirection);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.UserId > 0);
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.EmailAddress));
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.DisplayName));
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.OrganisationName));
            Assert.Contains(result, x => x.UserAccountStatusId >= 1);
            Assert.All(result, x => Assert.IsType<bool>(x.IsAdmin));
            Assert.All(result, x => Assert.IsType<bool>(x.IsPastLastLogonThreshold));
            Assert.All(result, x => Assert.IsType<bool>(x.MultiSearchEnabled));
        }

        [Theory]
        [InlineData("@nhs.net", SortBy.EmailAddress)]
        public void UsersFoundByEmailAddress(string emailAddress, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, emailAddress, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.EmailAddress.Contains(emailAddress));
        }

        [Theory]
        [InlineData("@gmail.com", SortBy.EmailAddress)]
        public void UsersNotFoundByEmailAddress(string emailAddress, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, emailAddress, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count == 0);
        }

        [Theory]
        [InlineData("HEALTH AND SOCIAL CARE INFORMATION CENTRE", SortBy.EmailAddress)]
        public void UsersFoundByOrganisationName(string organisationName, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, null, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.OrganisationName.Contains(organisationName));
        }

        [Theory]
        [InlineData("Gmail", SortBy.EmailAddress)]
        public void UsersNotFoundByOrganisationName(string organisationName, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, null, organisationName, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count == 0);
        }

        [Theory]
        [InlineData("A20047, A87456", "B72524, B27193", "1-June-2021:8-June-2021", "1 June 2021 13:17:18")]
        public void AddAndFindSearchGroup(string consumerOdsCodeInput, string providerOdsCodeInput, string searchDateRangeInput, string searchStartAt)
        {
            var result = AddSearchGroup(consumerOdsCodeInput, providerOdsCodeInput, searchDateRangeInput, searchStartAt);
            Assert.IsType<SearchGroup>(result);
            Assert.NotNull(result);
            Assert.Equal(result.ProviderOdsTextbox, providerOdsCodeInput);
            Assert.Equal(result.ConsumerOdsTextbox, consumerOdsCodeInput);
            Assert.Equal(result.SelectedDateRange, searchDateRangeInput);
            Assert.Equal(result.SearchStartAt, DateTime.Parse(searchStartAt));
            Assert.True(result.SearchGroupId > 0);
        }

        private SearchGroup AddSearchGroup(string consumerOdsCodeInput, string providerOdsCodeInput, string searchDateRangeInput, string searchStartAt)
        {
            return _applicationService.AddSearchGroup(new DTO.Request.Application.SearchGroup
            {
                ConsumerOdsTextbox = consumerOdsCodeInput,
                ProviderOdsTextbox = providerOdsCodeInput,
                SearchDateRange = searchDateRangeInput,
                SearchStartAt = DateTime.Parse(searchStartAt),
                UserSessionId = _user.UserSessionId
            });
        }

        [Theory]
        [InlineData("A37353", "B27181", 1, "Search details here", "EMIS", 0.237, "A37247, A99176", "C28888", "9-June-2021:16-June-2021", "12 April 2021 18:38:28")]
        public void AddAndFindSearchResult(string providerCode, string consumerCode, int errorCode, string details, string providerPublisher, double searchDurationSeconds, string consumerOdsCodeInput, string providerOdsCodeInput, string searchDateRangeInput, string searchStartAt)
        {
            var searchGroup = AddSearchGroup(consumerOdsCodeInput, providerOdsCodeInput, searchDateRangeInput, searchStartAt);

            var searchResult = new DTO.Request.Application.SearchResult
            {
                SearchGroupId = searchGroup.SearchGroupId,
                ProviderCode = providerCode,
                ConsumerCode = consumerCode,
                ErrorCode = errorCode,
                Details = details,
                ProviderPublisher = providerPublisher,
                SearchDurationSeconds = searchDurationSeconds
            };

            var result = _applicationService.AddSearchResult(searchResult);

            Assert.IsType<SearchResult>(result);
            Assert.NotNull(result);
            Assert.True(result.SearchGroupId > 0);
            Assert.True(result.SearchResultId > 0);

            var foundSearchResult = _applicationService.GetSearchResult(result.SearchResultId, _user.UserId);

            Assert.IsType<SearchResult>(foundSearchResult);
            Assert.NotNull(foundSearchResult);
            Assert.True(foundSearchResult.SearchGroupId > 0);
            Assert.True(foundSearchResult.SearchResultId > 0);
            Assert.Equal(foundSearchResult.ProviderOdsCode, providerCode);
            Assert.Equal(foundSearchResult.ConsumerOdsCode, consumerCode);
            Assert.Equal(foundSearchResult.SearchDurationSeconds, searchDurationSeconds);
            Assert.Equal(foundSearchResult.ProviderPublisher, providerPublisher);
        }

        [Theory]
        [InlineData("A37353", "B27181", 1, "Search details here", "EMIS", 0.237, "A37247, A99176", "C28888", "9-June-2021:16-June-2021", "12 April 2021 18:38:28")]
        public void GetSearchResultByGroup(string providerCode, string consumerCode, int errorCode, string details, string providerPublisher, double searchDurationSeconds, string consumerOdsCodeInput, string providerOdsCodeInput, string searchDateRangeInput, string searchStartAt)
        {
            var searchGroup = AddSearchGroup(consumerOdsCodeInput, providerOdsCodeInput, searchDateRangeInput, searchStartAt);
            var searchResult = new DTO.Request.Application.SearchResult
            {
                SearchGroupId = searchGroup.SearchGroupId,
                ProviderCode = providerCode,
                ConsumerCode = consumerCode,
                ErrorCode = errorCode,
                Details = details,
                ProviderPublisher = providerPublisher,
                SearchDurationSeconds = searchDurationSeconds
            };

            var result = _applicationService.AddSearchResult(searchResult);
            var foundSearchResultByGroup = _applicationService.GetSearchResultByGroup(searchGroup.SearchGroupId, _user.UserId);
            Assert.IsType<List<SlotEntrySummary>>(foundSearchResultByGroup);
            Assert.True(foundSearchResultByGroup.Count > 0);
            Assert.Contains(foundSearchResultByGroup, x => x.ProviderOdsCode == providerCode);
            Assert.Contains(foundSearchResultByGroup, x => x.ConsumerOdsCode == consumerCode);
            Assert.Contains(foundSearchResultByGroup, x => x.ProviderPublisher == providerPublisher);
            Assert.Contains(foundSearchResultByGroup, x => x.SearchSummaryDetail == details);
        }

        [Theory]
        [InlineData("user1@test.com", "User 1", 1, "Job Role 1", "Reason for wanting access")]
        public void AddAndSetUserAccountStatus(string emailAddress, string displayName, int organisationId, string jobRole, string reason)
        {
            var userCreateAccount = new DTO.Request.Application.UserCreateAccount
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                OrganisationId = organisationId,
                JobRole = jobRole,
                Reason = reason
            };
            _applicationService.AddOrUpdateUser(userCreateAccount);
            var user = _applicationService.GetUser(emailAddress);

            _applicationService.SetUserStatus(user.UserId, (int)UserAccountStatus.RequestDenied);
            user = _applicationService.GetUser(emailAddress);
            Assert.True(user.UserAccountStatusId == (int)UserAccountStatus.RequestDenied);

            _applicationService.SetUserStatus(user.UserId, (int)UserAccountStatus.Deauthorised);
            user = _applicationService.GetUser(emailAddress);
            Assert.True(user.UserAccountStatusId == (int)UserAccountStatus.Deauthorised);

            _applicationService.SetUserStatus(user.UserId, (int)UserAccountStatus.Authorised);
            user = _applicationService.GetUser(emailAddress);
            Assert.True(user.UserAccountStatusId == (int)UserAccountStatus.Authorised);
        }

        [Theory]
        [InlineData("user2@test.com", "User 2", 1, "Job Role 2", "Reason for wanting access is given here")]
        public void AddAndSetUserMultiSearch(string emailAddress, string displayName, int organisationId, string jobRole, string reason)
        {
            var userCreateAccount = new DTO.Request.Application.UserCreateAccount
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                OrganisationId = organisationId,
                JobRole = jobRole,
                Reason = reason
            };
            _applicationService.AddOrUpdateUser(userCreateAccount);
            var user = _applicationService.GetUser(emailAddress);
            
            _applicationService.SetMultiSearch(user.UserId, true);
            user = _applicationService.GetUser(emailAddress);
            Assert.True(user.MultiSearchEnabled);

            _applicationService.SetMultiSearch(user.UserId, false);
            user = _applicationService.GetUser(emailAddress);
            Assert.False(user.MultiSearchEnabled);
        }

        private static void SetupConfiguration(Mock<IConfiguration> mockConfiguration)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
        }

        private HttpContextAccessor SetupContextAccessor()
        {
            var httpContextAccessor = new HttpContextAccessor();
            return httpContextAccessor;
        }

        private static void SetupFluentMappings()
        {
            if (FluentMapper.EntityMaps.IsEmpty)
            {
                FluentMapper.Initialize(config =>
                {
                    config.AddMap(new UserMap());
                    config.AddMap(new OrganisationMap());
                    config.AddMap(new SearchResultMap());
                    config.AddMap(new SearchResultByGroupMap());
                    config.AddMap(new SearchGroupMap());
                    config.AddMap(new EmailTemplateMap());
                });
            }
        }

        private User AddUserDetailsToContext(HttpContextAccessor httpContextAccessor)
        {
            var adminUser = _applicationService.GetAdminUsers().FirstOrDefault();

            var logOnUser = new DTO.Request.Application.User
            {
                OrganisationId = adminUser.OrganisationId,
                DisplayName = adminUser.DisplayName,
                EmailAddress = adminUser.EmailAddress
            };

            var loggedOnAdminUser = _applicationService.LogonUser(logOnUser);

            IList<Claim> claimCollection = new List<Claim>
            {
                new Claim("UserId", loggedOnAdminUser.UserId.ToString()),
                new Claim("UserSessionId", loggedOnAdminUser.UserSessionId.ToString())
            };

            var identity = new GenericIdentity(loggedOnAdminUser.DisplayName, "TestAdminUser");
            identity.AddClaims(claimCollection);
            var contextUser = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext()
            {
                User = contextUser,
            };

            httpContextAccessor.HttpContext = httpContext;

            return loggedOnAdminUser;
        }
    }
}
