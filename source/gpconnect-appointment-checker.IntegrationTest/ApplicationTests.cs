using Dapper.FluentMap;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Application;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Mapping;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace gpconnect_appointment_checker.IntegrationTest
{

    [Collection("Sequential")]
    public class ApplicationTests
    {
        private readonly ApplicationService _applicationService;
        private readonly DataService _dataService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly User  _user;

        public ApplicationTests()
        {
            var mockLogger = new Mock<ILogger<ApplicationService>>();
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockLogService = new Mock<ILogService>();
            var mockAuditService = new Mock<IAuditService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            _mockHttpContextAccessor = SetupContextAccessor();

            SetupFluentMappings();
            SetupConfiguration(mockConfiguration);

            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
            _applicationService = new ApplicationService(mockConfiguration.Object, mockLogger.Object, _dataService, mockAuditService.Object, mockLogService.Object, _mockHttpContextAccessor.Object, mockEmailService.Object);

            _user = AddUserDetailsToContext(_mockHttpContextAccessor);
        }

        private User AddUserDetailsToContext(Mock<IHttpContextAccessor> mockHttpContextAccessor)
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

            mockHttpContextAccessor.Setup(a => a.HttpContext.User.Claims).Returns(claimCollection);

            return loggedOnAdminUser;
        }

        [Theory]
        [InlineData("A20047", "PR", "DR LEGG'S SURGERY", "LS1 4HY")]
        [InlineData("B82617", "PR", "COXWOLD SURGERY", "YO61 4BB")]
        public async void OrganisationFound(string odsCode, string organisationTypeCode, string organisationName, string postalCode)
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
        public async void OrganisationNotFound(string odsCode)
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
        public async void UsersFound(SortBy sortBy, SortDirection sortDirection)
        {
            var result = _applicationService.GetUsers(sortBy, sortDirection);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.UserId > 0);
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.EmailAddress));
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.DisplayName));
            Assert.Contains(result, x => !string.IsNullOrEmpty(x.OrganisationName));
            Assert.Contains(result, x => x.LastLogonDate <= DateTime.Now);
            Assert.Contains(result, x => x.UserAccountStatusId >= 1);
            Assert.All(result, x => Assert.IsType<bool>(x.IsAdmin));
            Assert.All(result, x => Assert.IsType<bool>(x.IsPastLastLogonThreshold));
            Assert.All(result, x => Assert.IsType<bool>(x.MultiSearchEnabled));
        }

        [Theory]
        [InlineData("@nhs.net", SortBy.EmailAddress)]
        public async void UsersFoundByEmailAddress(string emailAddress, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, emailAddress, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.EmailAddress.Contains(emailAddress));
        }

        [Theory]
        [InlineData("@gmail.com", SortBy.EmailAddress)]
        public async void UsersNotFoundByEmailAddress(string emailAddress, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, emailAddress, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count == 0);
        }

        [Theory]
        [InlineData("HEALTH AND SOCIAL CARE INFORMATION CENTRE", SortBy.EmailAddress)]
        public async void UsersFoundByOrganisationName(string organisationName, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, null, null, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count > 0);
            Assert.Contains(result, x => x.OrganisationName.Contains(organisationName));
        }

        [Theory]
        [InlineData("Gmail", SortBy.EmailAddress)]
        public async void UsersNotFoundByOrganisationName(string organisationName, SortBy sortBy)
        {
            var result = _applicationService.FindUsers(null, null, organisationName, sortBy);
            Assert.IsType<List<User>>(result);
            Assert.True(result.Count == 0);
        }

        [Theory]
        [InlineData("test@test.com", "Test User", 1, "A20047, A87456", "B72524, B27193", "1-June-2021:8-June-2021", "1 June 2021 13:17:18")]
        public async void AddAndFindSearchGroup(string emailAddress, string displayName, int organisationId, string consumerOdsCodeInput, string providerOdsCodeInput, string searchDateRangeInput, string searchStartAt)
        {
            var result = _applicationService.AddSearchGroup(new DTO.Request.Application.SearchGroup
            {
                ConsumerOdsTextbox = consumerOdsCodeInput,
                ProviderOdsTextbox = providerOdsCodeInput,
                SearchDateRange = searchDateRangeInput,
                SearchStartAt = DateTime.Parse(searchStartAt),
                UserSessionId = _user.UserSessionId
            });
            Assert.IsType<SearchGroup>(result);
            Assert.NotNull(result);
            Assert.Equal(result.ProviderOdsTextbox, providerOdsCodeInput);
            Assert.Equal(result.ConsumerOdsTextbox, consumerOdsCodeInput);
            Assert.Equal(result.SelectedDateRange, searchDateRangeInput);
            Assert.Equal(result.SearchStartAt, DateTime.Parse(searchStartAt));
            Assert.True(result.SearchGroupId > 0);
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

        private Mock<IHttpContextAccessor> SetupContextAccessor()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var contextUser = new ClaimsPrincipal(new ClaimsIdentity());
            httpContextAccessorMock.Setup(h => h.HttpContext.User).Returns(contextUser);
            return httpContextAccessorMock;
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
    }
}
