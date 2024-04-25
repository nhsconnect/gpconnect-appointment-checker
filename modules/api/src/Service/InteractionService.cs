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

    public async Task<string> CreateInteractionData(RouteReportRequest routeReportRequest)
    {
        try
        {
            var odsCodesInScope = routeReportRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(odsCodesInScope);
            var interactions = new List<IDictionary<string, object>>();

            if (odsCodesInScope.Count > 0)
            {
                for (var i = 0; i < odsCodesInScope.Count; i++)
                {
                    var interactionReporting = new InteractionReporting()
                    {
                        SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                        Hierarchy = organisationHierarchy[odsCodesInScope[i]],
                        DocumentsVersion = ActiveInactiveConstants.NOTAVAILABLE,
                        DocumentsInProfile = ActiveInactiveConstants.NOTAVAILABLE,
                        Profile = null,
                        StructuredVersion = ActiveInactiveConstants.NOTAVAILABLE
                    };

                    var interactionData = await GetInteractionData(routeReportRequest.Interaction[0], odsCodesInScope[i]);
                    if (interactionData != null && interactionData.NoIssues)
                    {
                        interactionReporting.Profile = interactionData.Profile;
                        interactionReporting.StructuredVersion = $"v{interactionData.Version}";
                    }

                    var interactionDataDocuments = await GetInteractionData(routeReportRequest.Interaction[1], odsCodesInScope[i]);
                    if (interactionDataDocuments != null && interactionDataDocuments.NoIssues)
                    {
                        interactionReporting.DocumentsVersion = $"v{interactionDataDocuments.Version}";
                        interactionReporting.DocumentsInProfile = interactionDataDocuments.Rest?.Count(x => x.Resource.Any(y => y.Type == "Binary")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
                    }

                    var jsonString = JsonConvert.SerializeObject(interactionReporting);
                    var jObject = JObject.Parse(jsonString);
                    interactions.Add(jObject.Flatten());
                }
                jsonData = JsonConvert.SerializeObject(interactions);
            }
            return jsonData.Substring(1, jsonData.Length - 2);
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
