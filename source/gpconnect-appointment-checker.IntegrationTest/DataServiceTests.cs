using Dapper;
using gpconnect_appointment_checker.DAL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace gpconnect_appointment_checker.IntegrationTest
{
    [Collection("ApplicationData")]
    public class DataServicePostApplicationTests
    {
        private readonly DataService _dataService;

        public DataServicePostApplicationTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
        }

        [Theory]
        [InlineData("application.synchronise_organisation", "ABC123", "GP", "Test Organisation 1", "Test Practice 1", "1 The Hill", "Higher Chuffing", "Taunton", "Somerset", "TA7 9SJ")]
        [InlineData("application.synchronise_organisation", "XYZ874", "PRAC", "Test Organisation 2", "Test Practice 2", "2 The Hill", "Higher Chuffing", "Taunton", "Somerset", "TA2 2HD")]
        public void PostSynchronisedOrganisationData(string functionName, string odsCode, string organisationTypeCode, string organisationName, string address1, string address2, string locality, string city, string county, string postCode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode);
            parameters.Add("_organisation_type_name", organisationTypeCode);
            parameters.Add("_organisation_name", organisationName);
            parameters.Add("_address_line_1", address1);
            parameters.Add("_address_line_2", address2);
            parameters.Add("_locality", locality);
            parameters.Add("_city", city);
            parameters.Add("_county", county);
            parameters.Add("_postcode", postCode);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("application.logon_user", "test@test.com", "Test User", 1)]
        [InlineData("application.logon_user", "test1@test1.com", "Test User 1", 1)]
        public void PostLogonUserData(string functionName, string emailAddress, string displayName, int organisationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", emailAddress);
            parameters.Add("_display_name", displayName);
            parameters.Add("_organisation_id", organisationId);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result); 
        }
    }

    [Collection("ApplicationData")]
    public class DataServiceGetApplicationTests
    {
        private readonly DataService _dataService;

        public DataServiceGetApplicationTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
        }

        [Theory]
        [InlineData("application.get_organisation", "XYZ372")]
        [InlineData("application.get_organisation", "924ABC")]
        [InlineData("application.get_organisation", "")]
        public void RetrieveInvalidOrganisationData(string functionName, string odsCode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("application.get_organisation", "ABC123")]
        [InlineData("application.get_organisation", "XYZ874")]
        public void RetrieveValidOrganisationData(string functionName, string odsCode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }
    }

    [Collection("ConfigurationData")]
    public class DataServiceGetConfigurationDataTests
    {
        private readonly DataService _dataService;

        public DataServiceGetConfigurationDataTests()
        {
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns("Server=localhost;Port=5432;Database=GpConnectAppointmentChecker;User Id=postgres;Password=hYrfbq74%Na$xFIe!QRA;");
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
        }

        [Theory]
        [InlineData("configuration.get_general_configuration")]
        [InlineData("configuration.get_sds_queries")]
        [InlineData("configuration.get_spine_configuration")]
        [InlineData("configuration.get_spine_message_type")]
        [InlineData("configuration.get_sso_configuration")]
        public void RetrieveConfigurationData(string functionName)
        {
            var result = _dataService.ExecuteFunction<object>(functionName);
            Assert.NotEmpty(result);
        }
    }

    [Collection("LoggingData")]
    public class DataServicePostLoggingTests
    {
        private readonly DataService _dataService;

        public DataServicePostLoggingTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object);
        }

        [Theory]
        [InlineData("logging.log_error", "gpconnect-appointment-checker, Version = 1.0.7618.18620, Culture = neutral, PublicKeyToken = null", "INFO", "An error has occurred - error 1", "Microsoft.Hosting.Lifetime", "Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync", "")]
        [InlineData("logging.log_error", "gpconnect-appointment-checker, Version = 1.0.7618.18620, Culture = neutral, PublicKeyToken = null", "INFO", "An error has occurred - error 2", "System.Net.Http.HttpClient.GpConnectClient.ClientHandler", "Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync", "")]
        [InlineData("logging.log_error", "gpconnect-appointment-checker, Version = 1.0.7618.18620, Culture = neutral, PublicKeyToken = null", "ERROR", "An error has occurred - error 3", "gpconnect_appointment_checker.GPConnect.GpConnectQueryExecutionService", "Microsoft.AspNetCore.Hosting.GenericWebHostService.StartAsync", "General Protection Fault")]
        public void PostErrorLoggingData(string functionName, string application, string level, string message, string logger, string callSite, string exception)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_application", application);
            parameters.Add("_logged", DateTime.UtcNow);
            parameters.Add("_level", level);
            parameters.Add("_user_id", null);
            parameters.Add("_user_session_id", null);
            parameters.Add("_message", message);
            parameters.Add("_logger", logger);
            parameters.Add("_callsite", callSite);
            parameters.Add("_exception", exception);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("logging.log_spine_message", 1, "ou=organisations, o=nhs (uniqueidentifier=B82617)", "{\"objectClass\":\"nhsGPPractice\",\"postalAddress\":\"COXWOLD$$$YORK$NORTH YORKSHIRE\",\"postalCode\":\"YO61 4BB\",\"nhsDHSCcode\":\"Y51\",\"nhsOrgOpenDate\":\"20080813\",\"nhsOrgSubType\":\"OC\",\"nhsPCTCode\":\"5B8\",\"nhsSyntheticIndicator\":\"0\",\"nhsOrgTypeCode\":\"PR\",\"nhsCountry\":\"England\",\"nhsSHAcode\":\"Q11\",\"uniqueIdentifier\":\"B82617\",\"telephoneNumber\":\"01347868426\",\"nhsIDCode\":\"B82617\",\"l\":\"NORTH YORKSHIRE\",\"nhsParentOrgCode\":\"5B8\",\"o\":\"COXWOLD SURGERY\",\"nhsJoinDate\":\"20080813\",\"nhsOrgType\":\"GP Practice\"}", 162)]
        [InlineData("logging.log_spine_message", 1, "ou=organisations, o=nhs (uniqueidentifier=A28371)", "{\"objectClass\":\"nhsGPPractice\",\"postalAddress\":\"COXWOLD$$$YORK$NORTH YORKSHIRE\",\"postalCode\":\"YO61 4BB\",\"nhsDHSCcode\":\"Y51\",\"nhsOrgOpenDate\":\"20080813\",\"nhsOrgSubType\":\"OC\",\"nhsPCTCode\":\"5B8\",\"nhsSyntheticIndicator\":\"0\",\"nhsOrgTypeCode\":\"PR\",\"nhsCountry\":\"England\",\"nhsSHAcode\":\"Q11\",\"uniqueIdentifier\":\"B82617\",\"telephoneNumber\":\"01347868426\",\"nhsIDCode\":\"B82617\",\"l\":\"NORTH YORKSHIRE\",\"nhsParentOrgCode\":\"5B8\",\"o\":\"COXWOLD SURGERY\",\"nhsJoinDate\":\"20080813\",\"nhsOrgType\":\"GP Practice\"}", 318)]
        public void PostSpineMessageLoggingData(string functionName, int messageTypeId, string requestPayload, string responsePayload, int roundTripTimeMs)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_user_session_id", null);
            parameters.Add("_spine_message_type_id", messageTypeId);
            parameters.Add("_command", null);
            parameters.Add("_request_headers", null);
            parameters.Add("_request_payload", requestPayload);
            parameters.Add("_response_status", null);
            parameters.Add("_response_headers", null);
            parameters.Add("_response_payload", responsePayload);
            parameters.Add("_roundtriptime_ms", roundTripTimeMs);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("logging.log_spine_message", 1, "Accept: application/fhir+json Ssp-From: 234290217329 Ssp-To: 925141112123 Ssp-InteractionID: urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1 Ssp - TraceID: 8c41972b-e583-40c5-8232-676caefd6aa4 Authorization: Bearer eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwicmVxdWVzdGVkX3Njb3BlIjoib3JnYW5pemF0aW9uLyoucmVhZCIsInN1YiI6ImJmYTMwNWFkLWFlYjgtNGJjNi04ZmE2LTkzZGQ0ZjJhM2YzNiIsInJlcXVlc3RpbmdfZGV2aWNlIjp7Im1vZGVsIjoiR1AgQ29ubmVjdCBBcHBvaW50bWVudCBDaGVja2VyIiwidmVyc2lvbiI6IjEuMC4wIiwicmVzb3VyY2VUeXBlIjoiRGV2aWNlIiwiaWRlbnRpZmllciI6W3sic3lzdGVtIjoiaHR0cHM6Ly9sb2NhbGhvc3QvU2VhcmNoIiwidmFsdWUiOiJsb2NhbGhvc3QifV19LCJyZXF1ZXN0aW5nX29yZ2FuaXphdGlvbiI6eyJuYW1lIjoiRFIgTEVHRydTIFNVUkdFUlkiLCJyZXNvdXJjZVR5cGUiOiJPcmdhbml6YXRpb24iLCJpZGVudGlmaWVyIjpbeyJzeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZSIsInZhbHVlIjoiQTIwMDQ3In1dfSwicmVxdWVzdGluZ19wcmFjdGl0aW9uZXIiOnsibmFtZSI6W3siZmFtaWx5IjoiLi4uIiwiZ2l2ZW4iOlsiLi4uIl0sInByZWZpeCI6bnVsbH1dLCJpZCI6ImJmYTMwNWFkLWFlYjgtNGJjNi04ZmE2LTkzZGQ0ZjJhM2YzNiIsInJlc291cmNlVHlwZSI6IlByYWN0aXRpb25lciIsImlkZW50aWZpZXIiOlt7InN5c3RlbSI6Imh0dHBzOi8vZmhpci5uaHMudWsvSWQvc2RzLXVzZXItaWQiLCJ2YWx1ZSI6IlVOSyJ9LHsic3lzdGVtIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkIiwidmFsdWUiOiJVTksifSx7InN5c3RlbSI6Imh0dHBzOi8vbG9jYWxob3N0L1NlYXJjaC91c2VyLWlkIiwidmFsdWUiOiJiZmEzMDVhZC1hZWI4LTRiYzYtOGZhNi05M2RkNGYyYTNmMzYifV19LCJleHAiOjE2MDM3OTg1MTMsImlhdCI6MTYwMzc5ODIxMywiaXNzIjoib3JhbmdlLnRlc3RsYWIubmhzLnVrIiwiYXVkIjoiaHR0cHM6Ly9vcmFuZ2UudGVzdGxhYi5uaHMudWsvZ3Bjb25uZWN0LWRlbW9uc3RyYXRvci92MS9maGlyIn0.", "Method: GET, RequestUri: 'https://orange.testlab.nhs.uk/gpconnect-demonstrator/v1/fhir/metadata', Version: 1.1, Content: <null>}", "OK", "Server: nginx", "{\"resourceType\":\"CapabilityStatement\", \"version\":\"1.2.7\", \"name\":\"GP Connect\", \"status\":\"active\", \"date\":\"2018-02-23\", \"publisher\":\"Not provided\", \"contact\":[{\"name\":\"NHS Digital\"}],\"description\":\"This server implements the GP Connect API version 1.2.7\",\"copyright\":\"Copyright NHS Digital 2018\",\"kind\":\"capability\",\"software\":{\"name\":\"Server\",\"version\":\"3.0.0\",\"releaseDate\":\"2017-09-27T00:00:00+01:00\"},\"Version\":\"3.0.1\",\"acceptUnknown\":\"both\",\"format\":[\"application/fhir+json\",\"application/fhir+xml\"],\"profile\":[{\"reference\":}", 162)]
        [InlineData("logging.log_spine_message", 1, "Accept: application/fhir+json Ssp-From: 326519300000 Ssp-To: 000033362232 Ssp-InteractionID: urn:nhs:names:services:gpconnect:fhir:rest:read:metadata-1 Ssp - TraceID: 8c41972b-e583-40c5-8232-676caefd6aa4 Authorization: Bearer eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJyZWFzb25fZm9yX3JlcXVlc3QiOiJkaXJlY3RjYXJlIiwicmVxdWVzdGVkX3Njb3BlIjoib3JnYW5pemF0aW9uLyoucmVhZCIsInN1YiI6ImJmYTMwNWFkLWFlYjgtNGJjNi04ZmE2LTkzZGQ0ZjJhM2YzNiIsInJlcXVlc3RpbmdfZGV2aWNlIjp7Im1vZGVsIjoiR1AgQ29ubmVjdCBBcHBvaW50bWVudCBDaGVja2VyIiwidmVyc2lvbiI6IjEuMC4wIiwicmVzb3VyY2VUeXBlIjoiRGV2aWNlIiwiaWRlbnRpZmllciI6W3sic3lzdGVtIjoiaHR0cHM6Ly9sb2NhbGhvc3QvU2VhcmNoIiwidmFsdWUiOiJsb2NhbGhvc3QifV19LCJyZXF1ZXN0aW5nX29yZ2FuaXphdGlvbiI6eyJuYW1lIjoiRFIgTEVHRydTIFNVUkdFUlkiLCJyZXNvdXJjZVR5cGUiOiJPcmdhbml6YXRpb24iLCJpZGVudGlmaWVyIjpbeyJzeXN0ZW0iOiJodHRwczovL2ZoaXIubmhzLnVrL0lkL29kcy1vcmdhbml6YXRpb24tY29kZSIsInZhbHVlIjoiQTIwMDQ3In1dfSwicmVxdWVzdGluZ19wcmFjdGl0aW9uZXIiOnsibmFtZSI6W3siZmFtaWx5IjoiLi4uIiwiZ2l2ZW4iOlsiLi4uIl0sInByZWZpeCI6bnVsbH1dLCJpZCI6ImJmYTMwNWFkLWFlYjgtNGJjNi04ZmE2LTkzZGQ0ZjJhM2YzNiIsInJlc291cmNlVHlwZSI6IlByYWN0aXRpb25lciIsImlkZW50aWZpZXIiOlt7InN5c3RlbSI6Imh0dHBzOi8vZmhpci5uaHMudWsvSWQvc2RzLXVzZXItaWQiLCJ2YWx1ZSI6IlVOSyJ9LHsic3lzdGVtIjoiaHR0cHM6Ly9maGlyLm5ocy51ay9JZC9zZHMtcm9sZS1wcm9maWxlLWlkIiwidmFsdWUiOiJVTksifSx7InN5c3RlbSI6Imh0dHBzOi8vbG9jYWxob3N0L1NlYXJjaC91c2VyLWlkIiwidmFsdWUiOiJiZmEzMDVhZC1hZWI4LTRiYzYtOGZhNi05M2RkNGYyYTNmMzYifV19LCJleHAiOjE2MDM3OTg1MTMsImlhdCI6MTYwMzc5ODIxMywiaXNzIjoib3JhbmdlLnRlc3RsYWIubmhzLnVrIiwiYXVkIjoiaHR0cHM6Ly9vcmFuZ2UudGVzdGxhYi5uaHMudWsvZ3Bjb25uZWN0LWRlbW9uc3RyYXRvci92MS9maGlyIn0.", "Method: GET, RequestUri: 'https://orange.testlab.nhs.uk/gpconnect-demonstrator/v1/fhir/metadata', Version: 1.1, Content: <null>}", "OK", "Server: nginx", "{\"resourceType\":\"CapabilityStatement\", \"version\":\"1.2.7\", \"name\":\"GP Connect\", \"status\":\"active\", \"date\":\"2018-02-23\", \"publisher\":\"Not provided\", \"contact\":[{\"name\":\"NHS Digital\"}],\"description\":\"This server implements the GP Connect API version 1.2.7\",\"copyright\":\"Copyright NHS Digital 2018\",\"kind\":\"capability\",\"software\":{\"name\":\"Server\",\"version\":\"3.0.0\",\"releaseDate\":\"2017-09-27T00:00:00+01:00\"},\"Version\":\"3.0.1\",\"acceptUnknown\":\"both\",\"format\":[\"application/fhir+json\",\"application/fhir+xml\"],\"profile\":[{\"reference\":}", 245)]
        public void PostSpineMessageLoggingToWithGpConnectRequestData(
            string functionName,
            int messageTypeId,
            string requestHeaders,
            string requestPayload,
            string responseStatus,
            string responseHeaders,
            string responsePayload, 
            int roundTripTimeMs)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_user_session_id", null);
            parameters.Add("_spine_message_type_id", messageTypeId);
            parameters.Add("_command", null);
            parameters.Add("_request_headers", requestHeaders);
            parameters.Add("_request_payload", requestPayload);
            parameters.Add("_response_status", responseStatus);
            parameters.Add("_response_headers", responseHeaders);
            parameters.Add("_response_payload", responsePayload);
            parameters.Add("_roundtriptime_ms", roundTripTimeMs);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("logging.log_web_request", "", "/search", "https://test.nhs.uk/home", "::1", "BLOGGS, Joe (NHS Digital)", "localhost", 200, "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36")]
        [InlineData("logging.log_web_request", "", "/privacypolicy", "https://test.nhs.uk/search", "::1", "FLINTSTONE, Fred (NHS Digital)", "localhost", 200, "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36")]
        public void PostWebRequestData(
            string functionName,
            string description,
            string url,
            string referrer,
            string ip,
            string createdBy,
            string server,
            int responseCode,
            string userAgent)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", null);
            parameters.Add("_user_session_id", null);
            parameters.Add("_url", url);
            parameters.Add("_referrer_url", referrer);
            parameters.Add("_description", description);
            parameters.Add("_ip", ip);
            parameters.Add("_created_date", DateTime.UtcNow);
            parameters.Add("_created_by", createdBy);
            parameters.Add("_server", server);
            parameters.Add("_response_code", responseCode);
            parameters.Add("_session_id", Guid.NewGuid().ToString());
            parameters.Add("_user_agent", userAgent);
            var result = _dataService.ExecuteFunction<object>(functionName, parameters);
            Assert.NotEmpty(result);
        }
    }

}
