using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Spine;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Api.Service.Fhir;

public class FhirService : IFhirService
{
    private readonly IConfigurationService _configurationService;
    private readonly IApplicationService _applicationService;
    private readonly IFhirRequestExecution _fhirRequestExecution;
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;
    private readonly ILogger<FhirService> _logger;

    public FhirService(ILogger<FhirService> logger, IApplicationService applicationService, IConfigurationService configurationService, IFhirRequestExecution fhirRequestExecution, IOptions<SpineConfig> spineOptionsDelegate)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _fhirRequestExecution = fhirRequestExecution ?? throw new ArgumentNullException(nameof(fhirRequestExecution));
        _spineOptionsDelegate = spineOptionsDelegate;
    }

    public async Task<Organisation> GetOrganisation(string odsCode)
    {
        if (!string.IsNullOrEmpty(odsCode))
        {
            var fhirApiQuery = await _configurationService.GetFhirApiQueryConfiguration(FhirQueryTypes.GetOrganisationDetailsByOdsCode.ToString());
            var organisation = await GetAndMapOrganisationResponse(odsCode, fhirApiQuery);
            await _applicationService.SynchroniseOrganisation(organisation);
            return organisation;
        }
        return null;
    }

    private async Task<Organisation> GetAndMapOrganisationResponse(string odsCode, FhirApiQuery fhirApiQuery)
    {
        var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) } });
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        var response = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Organisation.Organisation>(query, _spineOptionsDelegate.Value.SpineFhirApiDirectoryServicesFqdn, token, SpineMessageTypes.SpineFhirApiOrganisationQuery);

        if (response != null && !response.HasErrored && response.Issue == null)
        {
            var organisation = new Organisation
            {
                OdsCode = odsCode,
                OrganisationName = response.OrganisationName,
                OrganisationTypeCode = response.Type.Coding.OrganisationTypeDisplay,
                PostalAddress = response.PostalAddress.FullAddress,
                PostalCode = response.PostalAddress.PostCode
            };
            return organisation;
        }
        return null;
    }

    public async Task<Spine> GetProviderDetails(string odsCode, string interactionId)
    {
        var fhirApiQuery = await _configurationService.GetFhirApiQueryConfiguration(FhirQueryTypes.GetRoutingReliabilityDetailsFromSDS.ToString());
        var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) }, { "{interactionId}", Regex.Escape(interactionId) } });

        _logger.LogInformation("GetProviderDetails");
        _logger.LogInformation(query);

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        var spineProviderDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.Value.SpineFhirApiSystemsRegisterFqdn, token);

        if (spineProviderDetails != null && spineProviderDetails.Entries != null && spineProviderDetails.Entries.Count > 0)
        {
            var spineProviderAsId = await GetGpProviderAsIdByOdsCodeAndPartyKey(odsCode, spineProviderDetails.PartyKey);

            var spine = new Spine
            {
                PartyKey = spineProviderDetails.PartyKey,
                AsId = spineProviderAsId?.AsId,
                OdsCode = odsCode,
                ManufacturingOrganisationOdsCode = spineProviderAsId?.ManufacturingOrganisationOdsCode,
                EndpointAddress = spineProviderDetails.EndpointAddress,
                ProductName = await GetProductName(spineProviderAsId?.ManufacturingOrganisationOdsCode)
            };
            return spine;
        }
        return null;
    }

    public async Task<Spine> GetConsumerDetails(string odsCode)
    {
        if (!string.IsNullOrEmpty(odsCode))
        {
            var fhirApiQuery = await _configurationService.GetFhirApiQueryConfiguration(FhirQueryTypes.GetAccreditedSystemDetailsForConsumerFromSDS.ToString());
            var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) } });
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var spineConsumerDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.Value.SpineFhirApiSystemsRegisterFqdn, token);

            var spine = new Spine
            {
                AsId = spineConsumerDetails?.AsId,
                OdsCode = odsCode
            };
            return spine;
        }
        return null;
    }

    private async Task<string> GetProductName(string manufacturingOrganisationOdsCode)
    {
        var publisherOrganisation = await GetOrganisation(manufacturingOrganisationOdsCode);
        return publisherOrganisation?.OrganisationName;
    }

    private async Task<DTO.Response.Fhir.Spine> GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey)
    {
        var fhirApiQuery = await _configurationService.GetFhirApiQueryConfiguration(FhirQueryTypes.GetAccreditedSystemDetailsFromSDS.ToString());
        var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) },
                    { "{partyKey}", Regex.Escape(partyKey) }});
        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        var result = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.Value.SpineFhirApiSystemsRegisterFqdn, token);
        return result;
    }
}
