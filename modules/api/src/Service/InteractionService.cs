using DocumentFormat.OpenXml;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
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
    private readonly IOrganisationService _organisationService;
    private readonly IConfigurationService _configurationService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InteractionService(ILogger<InteractionService> logger, IConfigurationService configurationService, IMessageService messageService, IDataService dataService, ISpineService spineService, IOrganisationService organisationService, ICapabilityStatement capabilityStatement, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService;
        _spineService = spineService;
        _organisationService = organisationService;
        _configurationService = configurationService;
        _capabilityStatement = capabilityStatement;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> CreateInteractionData<T>(RouteReportRequest routeReportRequest) where T : class
    {
        try
        {
            var odsCodesInScope = routeReportRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(odsCodesInScope);
            var interactions = new List<IDictionary<string, object>>();
            string? jsonString = null;

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
                                Hierarchy = organisationHierarchy[odsCodesInScope[i]],
                                DocumentsVersion = ActiveInactiveConstants.NOTAVAILABLE,
                                DocumentsInProfile = ActiveInactiveConstants.NOTAVAILABLE,
                                Profile = null,
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var accessRecordStructuredReportingData = await GetInteractionData(routeReportRequest.Interaction[0], odsCodesInScope[i]);
                            if (accessRecordStructuredReportingData != null && accessRecordStructuredReportingData.NoIssues)
                            {
                                accessRecordStructuredReporting.Profile = accessRecordStructuredReportingData.Profile;
                                accessRecordStructuredReporting.ApiVersion = $"v{accessRecordStructuredReportingData.Version}";
                            }

                            var accessRecordStructuredReportingDataDocuments = await GetInteractionData(routeReportRequest.Interaction[1], odsCodesInScope[i]);
                            if (accessRecordStructuredReportingDataDocuments != null && accessRecordStructuredReportingDataDocuments.NoIssues)
                            {
                                accessRecordStructuredReporting.DocumentsVersion = $"v{accessRecordStructuredReportingDataDocuments.Version}";
                                accessRecordStructuredReporting.DocumentsInProfile = accessRecordStructuredReportingDataDocuments.Rest?.Count(x => x.Resource.Any(y => y.Type == "Binary")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
                            }

                            jsonString = JsonConvert.SerializeObject(accessRecordStructuredReporting);
                            break;

                        case Type type when type == typeof(AccessRecordHtmlReporting):

                            var accessRecordHtmlReporting = new AccessRecordHtmlReporting()
                            {
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = organisationHierarchy[odsCodesInScope[i]],
                                ApiVersion = ActiveInactiveConstants.NOTAVAILABLE
                            };

                            var accessRecordHtmlReportingData = await GetInteractionData(routeReportRequest.Interaction[0], odsCodesInScope[i]);
                            if (accessRecordHtmlReportingData != null && accessRecordHtmlReportingData.NoIssues)
                            {
                                accessRecordHtmlReporting.ApiVersion = $"v{accessRecordHtmlReportingData.Version}";
                            }

                            jsonString = JsonConvert.SerializeObject(accessRecordHtmlReporting);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var jObject = JObject.Parse(jsonString);
                    interactions.Add(jObject.Flatten());
                    jsonData = JsonConvert.SerializeObject(interactions);
                }
            }
            return jsonData != null ? jsonData.Substring(1, jsonData.Length - 2) : null;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateInteractionData'");
            throw;
        }
    }

    private async Task<CapabilityStatement?> GetInteractionData(string interaction, string odsCode)
    {
        var providerSpineDetails = await _spineService.GetProviderDetails(odsCode, interaction);

        if (providerSpineDetails != null)
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectReadMetaData, interaction);
            var requestParameters = await _tokenService.ConstructRequestParameters(new DTO.Request.GpConnect.RequestParameters()
            {
                RequestUri = new Uri($"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}"),
                ProviderSpineDetails = new SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId },
                ProviderOrganisationDetails = new OrganisationRequestParameters() { OdsCode = odsCode },
                SpineMessageTypeId = (SpineMessageTypes)spineMessageType.SpineMessageTypeId,
                Sid = Guid.NewGuid().ToString()
            });

            if (requestParameters != null)
            {
                var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname, interaction, TimeSpan.FromMinutes(2));
                return capabilityStatement;
            }
        }
        return null;
    }
}
