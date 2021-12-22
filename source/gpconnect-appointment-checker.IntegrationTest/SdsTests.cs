using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly SdsQueryExecutionBase _sdsQueryExecutionBase;
        private readonly DataService _dataService;

        public SdsTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLogger = new Mock<ILogger<SdsQueryExecutionBase>>();
            var mockLogService = new Mock<ILogService>();
            var mockLdapService = new Mock<ILdapService>();
            var mockFhirApiService = new Mock<IFhirApiService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSpineOptionsConfiguration = new Mock<IOptionsMonitor<Spine>>();
            var mockConfigurationSectionUseLdaps = new Mock<IConfigurationSection>();
            var mockConfigurationSectionTimeout = new Mock<IConfigurationSection>();
            var mockConfigurationSectionHost = new Mock<IConfigurationSection>();
            var mockConfigurationSectionPort = new Mock<IConfigurationSection>();

            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);

            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);

            SetupConfiguration(mockConfigurationSectionUseLdaps, mockConfigurationSectionTimeout, mockConfigurationSectionHost, mockConfigurationSectionPort, mockConfiguration);
            SetupContext(mockHttpContextAccessor);

            _sdsQueryExecutionBase = new SdsQueryExecutionBase(mockLogger.Object, mockLdapService.Object, mockFhirApiService.Object, mockSpineOptionsConfiguration.Object); ;
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

        private void SetupConfiguration(Mock<IConfigurationSection> mockConfigurationSectionUseLdaps, Mock<IConfigurationSection> mockConfigurationSectionTimeout,
            Mock<IConfigurationSection> mockConfigurationSectionHost, Mock<IConfigurationSection> mockConfigurationSectionPort, Mock<IConfiguration> mockConfiguration)
        {
            var configuration = _dataService.ExecuteFunction<Spine>("configuration.get_spine_configuration").FirstOrDefault();

            mockConfigurationSectionUseLdaps.Setup(a => a.Value).Returns(configuration.SdsUseLdaps.ToString());
            mockConfigurationSectionTimeout.Setup(a => a.Value).Returns(configuration.TimeoutSeconds.ToString());
            mockConfigurationSectionHost.Setup(a => a.Value).Returns(configuration.SdsHostname.ToString());
            mockConfigurationSectionPort.Setup(a => a.Value).Returns(configuration.SdsPort.ToString());

            mockConfiguration.Setup(a => a.GetSection("Spine:sds_use_ldaps")).Returns(mockConfigurationSectionUseLdaps.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:timeout_seconds")).Returns(mockConfigurationSectionTimeout.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_hostname")).Returns(mockConfigurationSectionHost.Object);
            mockConfiguration.Setup(a => a.GetSection("Spine:sds_port")).Returns(mockConfigurationSectionPort.Object);
        }
    }
}
