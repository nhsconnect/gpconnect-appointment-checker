using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ConfigurationService : IConfigurationService
{
    private readonly IDataService _dataService;

    public ConfigurationService(IDataService dataService)
    {
        _dataService = dataService ?? throw new ArgumentNullException();
    }

    public async Task<FhirApiQuery> GetFhirApiQueryConfiguration(string queryName)
    {
        var functionName = "configuration.get_fhir_api_queries";
        var result = await _dataService.ExecuteQuery<FhirApiQuery>(functionName);
        return result.FirstOrDefault(x => x.QueryName == queryName);
    }

    public async Task<IEnumerable<FhirApiQuery>> GetFhirApiQueryConfiguration()
    {
        var functionName = "configuration.get_fhir_api_queries";
        var result = await _dataService.ExecuteQuery<FhirApiQuery>(functionName);
        return result;
    }

    public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
    {
        var functionName = "configuration.get_organisation_type";
        var result = await _dataService.ExecuteQuery<OrganisationType>(functionName);
        return result;
    }

    public async Task<OrganisationType> GetOrganisationType(string organisationTypeCode)
    {
        var functionName = "configuration.get_organisation_type";
        var result = await _dataService.ExecuteQuery<OrganisationType>(functionName);
        return result.FirstOrDefault(x => x.OrganisationTypeCode == organisationTypeCode);
    }

    public async Task<SdsQuery> GetSdsQueryConfiguration(string queryName)
    {
        var functionName = "configuration.get_sds_queries";
        var result = await _dataService.ExecuteQuery<SdsQuery>(functionName);
        return result.FirstOrDefault(x => x.QueryName == queryName);
    }

    public async Task<IEnumerable<SdsQuery>> GetSdsQueryConfiguration()
    {
        var functionName = "configuration.get_sds_queries";
        var result = await _dataService.ExecuteQuery<SdsQuery>(functionName);
        return result;
    }

    public async Task<SpineMessageType> GetSpineMessageType(Helpers.Constants.SpineMessageTypes spineMessageType, string? interactionId = null)
    {
        var functionName = "configuration.get_spine_message_type";
        var result = await _dataService.ExecuteQuery<SpineMessageType>(functionName);
        if(interactionId != null)
        {
            return result.FirstOrDefault(x => x.InteractionId == interactionId);
        }
        return result.FirstOrDefault(x => x.SpineMessageTypeId == (int)spineMessageType);
    }

    public async Task<IEnumerable<SpineMessageType>> GetSpineMessageTypes()
    {
        var functionName = "configuration.get_spine_message_type";
        var result = await _dataService.ExecuteQuery<SpineMessageType>(functionName);
        return result;
    }
}
