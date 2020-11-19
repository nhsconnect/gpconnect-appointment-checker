using System;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace gpconnect_appointment_checker.IntegrationTest
{
    public class GpConnectTests
    {
        private readonly GpConnectQueryExecutionService _gpConnectQueryExecutionService;

        public GpConnectTests()
        {
            var mockLogger = new Mock<ILogger<GpConnectQueryExecutionService>>();
            var mockLogService = new Mock<ILogService>();
            var mockConfigurationService = new Mock<IConfigurationService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();

            SetupSpineMessageTypes(mockConfigurationService);
            SetupSdsQueries(mockConfigurationService);
            SetupContextAccessor(mockHttpContextAccessor);
            SetupHttpClient(mockHttpClientFactory);
            SetupConfiguration(mockConfiguration);

            _gpConnectQueryExecutionService = new GpConnectQueryExecutionService(mockLogger.Object, mockConfigurationService.Object, mockLogService.Object, mockHttpClientFactory.Object, mockHttpContextAccessor.Object);
        }

        [Theory]
        [InlineData("ABC123", "82734", "28374", "hostname", false, "A32874", "B28373", "DKJCH8943NJFSADV", 2, "https://test.hscic.gov.uk:19192/v1/fhir")]
        public async void ExecuteRequestForCapabilityStatement(string bearerToken, string sspFrom, string sspTo, string sspHostname, bool useSSP, string providerOdsCode, string consumerOdsCode, string interactionId, int spineMessageTypeId, string baseAddress)
        {
            var requestParameters = CreateRequestParameters(bearerToken, sspFrom, sspTo, sspHostname, useSSP, providerOdsCode, consumerOdsCode, interactionId, spineMessageTypeId);
            var result = await _gpConnectQueryExecutionService.ExecuteFhirCapabilityStatement(requestParameters, baseAddress);
            Assert.IsType<CapabilityStatement>(result);
            Assert.Equal("CapabilityStatement", result.ResourceType);
            Assert.Equal("1.2.7", result.Version);
            Assert.Equal("GP Connect", result.Name);
            Assert.Equal("active", result.status);
        }

        [Theory]
        [InlineData("ABC123", "82734", "28374", "hostname", false, "A32874", "B28373", "DKJCH8943NJFSADV", 2, "https://test.hscic.gov.uk:19192/v1/fhir")]
        public async void ExecuteRequestForFreeSlots(string bearerToken, string sspFrom, string sspTo, string sspHostname, bool useSSP, string providerOdsCode, string consumerOdsCode, string interactionId, int spineMessageTypeId, string baseAddress)
        {
            var requestParameters = CreateRequestParameters(bearerToken, sspFrom, sspTo, sspHostname, useSSP, providerOdsCode, consumerOdsCode, interactionId, spineMessageTypeId);
            var result = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearch(requestParameters, DateTime.Now, DateTime.Now.AddDays(7), baseAddress);
            Assert.IsType<SlotSimple>(result);
        }

        private static RequestParameters CreateRequestParameters(string bearerToken, string sspFrom, string sspTo,
            string sspHostname, bool useSSP, string providerOdsCode, string consumerOdsCode, string interactionId,
            int spineMessageTypeId)
        {
            var requestParameters = new RequestParameters();
            requestParameters.ProviderODSCode = providerOdsCode;
            requestParameters.ConsumerODSCode = consumerOdsCode;
            requestParameters.SpineMessageTypeId = spineMessageTypeId;
            requestParameters.InteractionId = interactionId;
            requestParameters.UseSSP = useSSP;
            requestParameters.BearerToken = bearerToken;
            requestParameters.SspFrom = sspFrom;
            requestParameters.SspTo = sspTo;
            requestParameters.SspHostname = sspHostname;
            return requestParameters;
        }

        private static void SetupConfiguration(Mock<IConfiguration> mockConfiguration)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings")))
                .Returns(mockConfSection.Object);
        }

        private static void SetupContextAccessor(Mock<IHttpContextAccessor> mockHttpContextAccessor)
        {
            var context = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        }

        private static void SetupHttpClient(Mock<IHttpClientFactory> mockHttpClientFactory)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "{\"resourceType\":\"CapabilityStatement\",\"version\":\"1.2.7\",\"name\":\"GP Connect\",\"status\":\"active\"}")
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient).Verifiable();
        }

        private static void SetupSdsQueries(Mock<IConfigurationService> mockConfigurationService)
        {
            var sdsQueries = new List<SdsQuery>()
            {
                new SdsQuery
                {
                    SearchBase = "ou=organisations, o=nhs",
                    QueryText = "(uniqueidentifier={odsCode})",
                    QueryName = "GetOrganisationDetailsByOdsCode"
                },
                new SdsQuery
                {
                    SearchBase = "ou=services, o=nhs",
                    QueryText =
                        "(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))",
                    QueryName = "GetGpProviderEndpointAndPartyKeyByOdsCode"
                },
                new SdsQuery
                {
                    SearchBase = "ou=services, o=nhs",
                    QueryText = "(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsMhsPartyKey={partyKey}))",
                    QueryName = "GetGpProviderAsIdByOdsCodeAndPartyKey"
                }
            };

            mockConfigurationService.Setup(a => a.GetSdsQueryConfiguration()).ReturnsAsync(sdsQueries);
        }

        private static void SetupSpineMessageTypes(Mock<IConfigurationService> mockConfigurationService)
        {
            var spineMessageTypes = new List<SpineMessageType>()
            {
                new SpineMessageType
                {
                    SpineMessageTypeId = 1,
                    SpineMessageTypeName = "Spine Directory Service - LDAP query",
                    InteractionId = "urn:nhs:names:services:sds:ldap"
                },
                new SpineMessageType
                {
                    SpineMessageTypeId = 2,
                    SpineMessageTypeName = "GP Connect - Read metadata (Appointment Management)",
                    InteractionId = "urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1"
                },
                new SpineMessageType
                {
                    SpineMessageTypeId = 3,
                    SpineMessageTypeName = "GP Connect - Search for free slots",
                    InteractionId = "urn:nhs:names:services:gpconnect:fhir:rest:search:slot-1"
                }
            };
            mockConfigurationService.Setup(a => a.GetSpineMessageTypes()).ReturnsAsync(spineMessageTypes);
        }
    }
}
