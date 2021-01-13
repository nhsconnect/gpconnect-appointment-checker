using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace gpconnect_appointment_checker.IntegrationTest
{
    public class SdsTests
    {
        private readonly SDSQueryExecutionService _sdsQueryExecutionService;
        private readonly DataService _dataService;

        public SdsTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLogger = new Mock<ILogger<SDSQueryExecutionService>>();
            var mockLogService = new Mock<ILogService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var mockConfiguration = new Mock<IConfiguration>();
            var mockConfigurationSectionUseLdaps = new Mock<IConfigurationSection>();
            var mockConfigurationSectionMutualAuth = new Mock<IConfigurationSection>();
            var mockConfigurationSectionTimeout = new Mock<IConfigurationSection>();
            var mockConfigurationSectionHost = new Mock<IConfigurationSection>();
            var mockConfigurationSectionPort = new Mock<IConfigurationSection>();

            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);

            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);

            SetupConfiguration(mockConfigurationSectionMutualAuth, mockConfigurationSectionUseLdaps, mockConfigurationSectionTimeout, mockConfigurationSectionHost, mockConfigurationSectionPort, mockConfiguration);
            SetupContext(mockHttpContextAccessor);

            _sdsQueryExecutionService = new SDSQueryExecutionService(mockLogger.Object, mockLogService.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);
        }

        [Theory]
        [InlineData("ou=organisations, o=nhs", "(uniqueidentifier=A20047)", new [] { "o", "postalCode" })]
        public void ExecuteValidQuery(string searchBase, string filter, string[] attributes)
        {
            var results = _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter, attributes);
            Assert.NotEmpty(results);
        }

        [Theory]
        [InlineData("ou=organisations, o=nhs", "(17256)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=organisations, o=nhs", "(A20047)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=organisations, o=nhs", "(*)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        public void ExecuteFilterExceptionQuery(string searchBase, string filter, string[] attributes)
        {
            Assert.Throws<LdapLocalException>(() => _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter, attributes));
        }

        [Theory]
        [InlineData("ou=organisations, o=nhs", "(uniqueidentifier=XYZ72615)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=organisations, o=nhs", "(nonexistentfilter=ABC123)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        public void ExecuteEmptyQuery(string searchBase, string filter, string[] attributes)
        {
            var results = _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter, attributes);
            Assert.Null(results);
        }

        [Theory]
        [InlineData("ou=organisations", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=fakeou", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=organisations, o=fakeobject", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("o=nhs", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("", "(uniqueidentifier=A20047)", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        [InlineData("ou=organisations, o=nhs", "", new[] { "nhsIDCode", "o", "postalAddress", "postalCode", "nhsOrgTypeCode" })]
        public void ExecuteExceptionQuery(string searchBase, string filter, string[] attributes)
        {
            Assert.Throws<LdapException>(() => _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter, attributes));
        }

        private static void SetupContext(Mock<IHttpContextAccessor> mockHttpContextAccessor)
        {
            var context = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim("UserSessionId", "1"),
                new Claim("UserId", "1"),
                new Claim("Email", "abc@test.com"),
                new Claim("DisplayName", "Test User"),
                new Claim("OrganisationName", "Test User Organisation"),
                new Claim("ProviderODSCode", "A9876")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            context.User = new ClaimsPrincipal(identity);
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        }

        private void SetupConfiguration(Mock<IConfigurationSection> mockConfigurationSectionMutualAuth, Mock<IConfigurationSection> mockConfigurationSectionUseLdaps, Mock<IConfigurationSection> mockConfigurationSectionTimeout,
            Mock<IConfigurationSection> mockConfigurationSectionHost, Mock<IConfigurationSection> mockConfigurationSectionPort, Mock<IConfiguration> mockConfiguration)
        {
            var configuration = _dataService.ExecuteFunction<Spine>("configuration", "get_spine_configuration").FirstOrDefault();

            mockConfigurationSectionUseLdaps.Setup(a => a.Value).Returns(configuration.sds_use_ldaps.ToString());
            mockConfigurationSectionMutualAuth.Setup(a => a.Value).Returns(configuration.sds_use_mutualauth.ToString());
            mockConfigurationSectionTimeout.Setup(a => a.Value).Returns(configuration.timeout_seconds.ToString());
            mockConfigurationSectionHost.Setup(a => a.Value).Returns(configuration.sds_hostname);
            mockConfigurationSectionPort.Setup(a => a.Value).Returns(configuration.sds_port.ToString());

            mockConfiguration.Setup(a => a.GetSection("Spine:sds_use_ldaps")).Returns(mockConfigurationSectionUseLdaps.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_use_mutualauth")).Returns(mockConfigurationSectionMutualAuth.Object); 
            mockConfiguration.Setup(a => a.GetSection("Spine:timeout_seconds")).Returns(mockConfigurationSectionTimeout.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_hostname")).Returns(mockConfigurationSectionHost.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_port")).Returns(mockConfigurationSectionPort.Object);
        }
    }
}
