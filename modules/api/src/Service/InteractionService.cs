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
    private readonly IReportingTokenService _reportingTokenService;
    private readonly ISpineService _spineService;
    private readonly IConfigurationService _configurationService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InteractionService(ILogger<InteractionService> logger, IConfigurationService configurationService, ISpineService spineService, ICapabilityStatement capabilityStatement, ITokenService tokenService, IReportingTokenService reportingTokenService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reportingTokenService = reportingTokenService ?? throw new ArgumentNullException(nameof(reportingTokenService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _spineService = spineService ?? throw new ArgumentNullException(nameof(spineService)); ;
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _capabilityStatement = capabilityStatement ?? throw new ArgumentNullException(nameof(capabilityStatement));
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
                                OdsCode = odsCodesInScope[i],
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy,
                                DocumentsVersion = ActiveInactiveConstants.NOTAVAILABLE,
                                DocumentsInProfile = ActiveInactiveConstants.NOTAVAILABLE,
                                Profile = null,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var accessRecordStructuredReportingData = await GetInteractionData(new InteractionDataRequest
                            {
                                OdsCode = odsCodesInScope[i],
                                Interaction = routeReportRequest.Interaction[0],
                                Client = Clients.GPCONNECTCLIENT,
                                HostIdentifier = "https://fhir.nhs.uk",
                                HasId = true
                            });

                            if (accessRecordStructuredReportingData != null && accessRecordStructuredReportingData.NoIssues)
                            {
                                accessRecordStructuredReporting.Profile = accessRecordStructuredReportingData.Profile;
                                accessRecordStructuredReporting.ApiVersion = $"v{accessRecordStructuredReportingData.Version}";
                            }

                            var accessRecordStructuredReportingDataDocuments = await GetInteractionData(new InteractionDataRequest
                            {
                                OdsCode = odsCodesInScope[i],
                                Interaction = routeReportRequest.Interaction[1],
                                Client = Clients.GPCONNECTCLIENT,
                                HostIdentifier = "https://fhir.nhs.uk",
                                HasId = true
                            });

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

                            var accessRecordHtmlReporting = new AccessRecordHtmlReporting()
                            {
                                OdsCode = odsCodesInScope[i],
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var accessRecordHtmlReportingData = await GetReportingInteractionData(new InteractionDataRequest()
                            {
                                OdsCode = odsCodesInScope[i],
                                Interaction = routeReportRequest.Interaction[0],
                                Client = Clients.GPCONNECTCLIENTLEGACY,
                                HostIdentifier = "http://fhir.nhs.net",
                                AuthenticationAudience = "https://authorize.fhir.nhs.net/token",
                                HasId = false
                            });

                            if (accessRecordHtmlReportingData != null && accessRecordHtmlReportingData.NoIssues)
                            {
                                accessRecordHtmlReporting.Rest = accessRecordHtmlReportingData.Rest;
                                accessRecordHtmlReporting.ApiVersion = $"v{accessRecordHtmlReportingData.Version}";
                            }

                            var jsonStringARH = JsonConvert.SerializeObject(accessRecordHtmlReporting);
                            var jObjectARH = JObject.Parse(jsonStringARH);
                            var jDictObjARH = jObjectARH.Flatten();

                            interactions.Add(jDictObjARH);
                            break;
                        case Type type when type == typeof(AppointmentManagementReporting):

                            var appointmentManagementReporting = new AppointmentManagementReporting()
                            {
                                OdsCode = odsCodesInScope[i],
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var appointmentManagementReportingData = await GetInteractionData(new InteractionDataRequest()
                            {
                                OdsCode = odsCodesInScope[i],
                                Interaction = routeReportRequest.Interaction[0],
                                Client = Clients.GPCONNECTCLIENT,
                                HostIdentifier = "https://fhir.nhs.uk",
                                HasId = true
                            });

                            if (appointmentManagementReportingData != null && appointmentManagementReportingData.NoIssues)
                            {
                                appointmentManagementReporting.Rest = appointmentManagementReportingData.Rest;
                                appointmentManagementReporting.ApiVersion = $"v{appointmentManagementReportingData.Version}";
                            }

                            var jsonStringAM = JsonConvert.SerializeObject(appointmentManagementReporting);
                            var jObjectAM = JObject.Parse(jsonStringAM);
                            var jDictObjAM = jObjectAM.Flatten();

                            interactions.Add(jDictObjAM);
                            break;
                    }
                }
                jsonData = JsonConvert.SerializeObject(interactions);
            }            
            return jsonData;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateInteractionData'");
            throw;
        }
    }

    private async Task<CapabilityStatement?> GetInteractionData(InteractionDataRequest interactionDataRequest)
    {
        var providerSpineDetails = await _spineService.GetProviderDetails(interactionDataRequest.OdsCode, interactionDataRequest.Interaction);        

        if (providerSpineDetails != null)
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, interactionDataRequest.Interaction);
            var requestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}");
            var spineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId };
            var organisationDetails = new OrganisationRequestParameters() { OdsCode = interactionDataRequest.OdsCode };

            var input = new DTO.Request.GpConnect.RequestParameters
            {
                RequestUri = requestUri,
                ProviderSpineDetails = spineDetails,
                ProviderOrganisationDetails = organisationDetails,
                SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId,
                Sid = Guid.NewGuid().ToString(),
                SystemIdentifier = interactionDataRequest.SystemIdentifier,
                AuthenticationAudience = interactionDataRequest.AuthenticationAudience,
                HostIdentifier = interactionDataRequest.HostIdentifier
            };

            var requestParameters = await _tokenService.ConstructRequestParameters(input, null);

            if (requestParameters != null)
            {
                var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname, interactionDataRequest.Client, interactionDataRequest.Interaction, TimeSpan.FromMinutes(3));
                return capabilityStatement;
            }
        }
        return null;
    }

    private async Task<CapabilityStatement?> GetReportingInteractionData(InteractionDataRequest interactionDataRequest)
    {
        var providerSpineDetails = await _spineService.GetProviderDetails(interactionDataRequest.OdsCode, interactionDataRequest.Interaction);

        if (providerSpineDetails != null)
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, interactionDataRequest.Interaction);
            var requestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}");
            var spineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId };
            var organisationDetails = new OrganisationRequestParameters() { OdsCode = interactionDataRequest.OdsCode };

            var input = new DTO.Request.GpConnect.RequestParameters
            {
                RequestUri = requestUri,
                ProviderSpineDetails = spineDetails,
                ProviderOrganisationDetails = organisationDetails,
                SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId,
                Sid = Guid.NewGuid().ToString(),
                SystemIdentifier = interactionDataRequest.SystemIdentifier,
                AuthenticationAudience = interactionDataRequest.AuthenticationAudience,
                HostIdentifier = interactionDataRequest.HostIdentifier
            };

            var requestParameters = await _reportingTokenService.ConstructRequestParameters(input, null, interactionDataRequest.HasId);

            if (requestParameters != null)
            {
                var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname, interactionDataRequest.Client, interactionDataRequest.Interaction, TimeSpan.FromMinutes(3));
                return capabilityStatement;
            }
        }
        return null;
    }
}
