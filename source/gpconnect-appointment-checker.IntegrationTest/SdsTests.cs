using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.SDS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace gpconnect_appointment_checker.IntegrationTest
{
    public class SdsTests
    {
        private readonly SDSQueryExecutionService _sdsQueryExecutionService;

        public SdsTests()
        {
            var mockLogger = new Mock<ILogger<SDSQueryExecutionService>>();
            var mockLogService = new Mock<ILogService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var mockConfiguration = new Mock<IConfiguration>();
            var mockConfigurationSectionUseLdaps = new Mock<IConfigurationSection>();
            var mockConfigurationSectionTimeout = new Mock<IConfigurationSection>();
            var mockConfigurationSectionHost = new Mock<IConfigurationSection>();
            var mockConfigurationSectionPort = new Mock<IConfigurationSection>();

            mockConfigurationSectionUseLdaps.Setup(a => a.Value).Returns("true");
            mockConfigurationSectionTimeout.Setup(a => a.Value).Returns("1000");
            mockConfigurationSectionHost.Setup(a => a.Value).Returns("orange.testlab.nhs.uk");
            mockConfigurationSectionPort.Setup(a => a.Value).Returns("636");

            mockConfiguration.Setup(a => a.GetSection("Spine:sds_use_ldaps")).Returns(mockConfigurationSectionUseLdaps.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:timeout_seconds")).Returns(mockConfigurationSectionTimeout.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_hostname")).Returns(mockConfigurationSectionHost.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_port")).Returns(mockConfigurationSectionPort.Object);

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

            _sdsQueryExecutionService = new SDSQueryExecutionService(mockLogger.Object, mockLogService.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);

        }

        [Theory]
        [InlineData("ou=organisations, o=nhs", "(uniqueidentifier=A20047)")]
        [InlineData("", "")]
        [InlineData("", "(uniqueidentifier=A20047)")]
        [InlineData("ou=organisations, o=nhs", "")]
        public void ExecuteValidQuery(string searchBase, string filter)
        {
            var results = _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter);
            Assert.NotEmpty(results);
        }

        [Theory]
        [InlineData("ou=organisations", "")]
        [InlineData("ou=fakeou", "")]
        [InlineData("ou=organisations, o=fakeobject", "")]
        public void ExecuteExceptionQuery(string searchBase, string filter)
        {
            Assert.Throws<Novell.Directory.Ldap.LdapException>(() => _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter));
        }

        [Theory]
        [InlineData("ou=organisations, o=nhs", "(uniqueidentifier=A20047)")]
        [InlineData("", "")]
        [InlineData("", "(uniqueidentifier=A20047)")]
        [InlineData("ou=organisations, o=nhs", "")]
        public void ExecuteQueryWithInvalidConnection(string searchBase, string filter)
        {
            var results = _sdsQueryExecutionService.ExecuteLdapQuery<Dictionary<string, object>>(searchBase, filter);
            Assert.NotEmpty(results);
        }
    }
}
