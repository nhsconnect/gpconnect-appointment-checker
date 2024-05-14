using DocumentFormat.OpenXml;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using gpconnect_appointment_checker.api.DTO.Response.Reporting;
using JsonFlatten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq.Dynamic.Core;

namespace GpConnect.AppointmentChecker.Api.Service;

public class InteractionService : IInteractionService
{
    private readonly ILogger<InteractionService> _logger;
    private readonly ITokenService _tokenService;
    private readonly ISpineService _spineService;
    private readonly IConfigurationService _configurationService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InteractionService(ILogger<InteractionService> logger, IConfigurationService configurationService, ISpineService spineService, ICapabilityStatement capabilityStatement, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService;
        _spineService = spineService;
        _configurationService = configurationService;
        _capabilityStatement = capabilityStatement;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<string?> CreateInteractionData<T>(RouteReportRequest routeReportRequest) where T : class
    {
        try
        {
            var odsCodesInScope = routeReportRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var interactions = new List<IDictionary<string, object>>();

            if (odsCodesInScope.Count > 0)
            {
                for (var i = 0; i < odsCodesInScope.Count; i++)
                {
                    switch (typeof(T))
                    {
                        case Type type when type == typeof(AccessRecordStructuredReporting):

                            var accessRecordStructuredReporting = new AccessRecordStructuredReporting()
                            {
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy,
                                DocumentsVersion = ActiveInactiveConstants.NOTAVAILABLE,
                                DocumentsInProfile = ActiveInactiveConstants.NOTAVAILABLE,
                                Profile = null,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var accessRecordStructuredReportingData = await GetInteractionData(routeReportRequest.Interaction[0], odsCodesInScope[i], Clients.GPCONNECTCLIENT, "https://fhir.nhs.uk/Id/ods-organization-code", "https://fhir.nhs.uk", null, true);

                            if (accessRecordStructuredReportingData != null && accessRecordStructuredReportingData.NoIssues)
                            {
                                accessRecordStructuredReporting.Profile = accessRecordStructuredReportingData.Profile;
                                accessRecordStructuredReporting.ApiVersion = $"v{accessRecordStructuredReportingData.Version}";
                            }

                            var accessRecordStructuredReportingDataDocuments = await GetInteractionData(routeReportRequest.Interaction[1], odsCodesInScope[i], Clients.GPCONNECTCLIENT, "https://fhir.nhs.uk/Id/ods-organization-code", "https://fhir.nhs.uk", null, true);
                            if (accessRecordStructuredReportingDataDocuments != null && accessRecordStructuredReportingDataDocuments.NoIssues)
                            {
                                accessRecordStructuredReporting.DocumentsVersion = $"v{accessRecordStructuredReportingDataDocuments.Version}";
                                accessRecordStructuredReporting.DocumentsInProfile = accessRecordStructuredReportingDataDocuments.Rest?.Count(x => x.Resource.Any(y => y.Type == "Binary")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
                            }

                            var jsonStringARS = JsonConvert.SerializeObject(accessRecordStructuredReporting);
                            var jObjectARS = JObject.Parse(jsonStringARS);
                            var jDictObjARS = jObjectARS.Flatten();

                            interactions.Add(jDictObjARS);
                            break;

                        case Type type when type == typeof(AccessRecordHtmlReporting):

                            _logger.LogInformation($"In AccessRecordHtmlReporting");

                            var accessRecordHtmlReporting = new AccessRecordHtmlReporting()
                            {
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            _logger.LogInformation($"In AccessRecordHtmlReporting routeReportRequest.Interaction[0] is {routeReportRequest.Interaction[0]}");
                            _logger.LogInformation($"In AccessRecordHtmlReporting odsCodesInScope[i] is {odsCodesInScope[i]}");

                            var accessRecordHtmlReportingData = await GetInteractionData(routeReportRequest.Interaction[0], odsCodesInScope[i], Clients.GPCONNECTCLIENTLEGACY, "http://fhir.nhs.net/Id/ods-organization-code", "http://fhir.nhs.net", "https://authorize.fhir.nhs.net/token", false);
                            if (accessRecordHtmlReportingData != null && accessRecordHtmlReportingData.NoIssues)
                            {
                                _logger.LogInformation($"In AccessRecordHtmlReporting accessRecordHtmlReportingData is not null");
                                _logger.LogInformation($"In AccessRecordHtmlReporting Rest.Count is {accessRecordHtmlReportingData.Rest.Count}");

                                foreach (var rest in accessRecordHtmlReportingData.Rest)
                                {
                                    _logger.LogInformation($"In AccessRecordHtmlReporting rest.Operation.Count is {rest.Operation.Count}");
                                    foreach (var item in rest?.Operation)
                                    {
                                        _logger.LogInformation($"In AccessRecordHtmlReporting accessRecordHtmlReportingData.Rest.Operation.Name is {item.Name}");
                                    }
                                }
                                accessRecordHtmlReporting.ApiVersion = $"v{accessRecordHtmlReportingData.Version}";
                            }

                            var jsonStringARH = JsonConvert.SerializeObject(accessRecordHtmlReporting);
                            var jObjectARH = JObject.Parse(jsonStringARH);
                            var jDictObjARH = jObjectARH.Flatten();

                            interactions.Add(jDictObjARH);
                            break;
                    }
                }
                jsonData = JsonConvert.SerializeObject(interactions);
            }
            return jsonData != null ? jsonData.Substring(1, jsonData.Length - 2) : null;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateInteractionData'");
            throw;
        }
    }

    private async Task<CapabilityStatement?> GetInteractionData(string interaction, string odsCode, string client, string systemIdentifier, string hostIdentifier, string? authenticationAudience = null, bool isID = true)
    {
        var providerSpineDetails = await _spineService.GetProviderDetails(odsCode, interaction);        

        if (providerSpineDetails != null)
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, interaction);
            var requestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}");
            var spineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId };
            var organisationDetails = new OrganisationRequestParameters() { OdsCode = odsCode };

            var input = new DTO.Request.GpConnect.RequestParameters
            {
                RequestUri = requestUri,
                ProviderSpineDetails = spineDetails,
                ProviderOrganisationDetails = organisationDetails,
                SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId,
                Sid = Guid.NewGuid().ToString(),
                SystemIdentifier = systemIdentifier,
                AuthenticationAudience = authenticationAudience,
                HostIdentifier = hostIdentifier                
            };

            var requestParameters = await _tokenService.ConstructRequestParameters(input, null, isID);

            if (requestParameters != null)
            {
                var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname, client, interaction, TimeSpan.FromMinutes(3));
                return capabilityStatement;
            }
        }
        return null;
    }
}
