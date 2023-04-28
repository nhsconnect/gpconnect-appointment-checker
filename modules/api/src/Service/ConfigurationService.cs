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

    public async Task<Email> GetEmailConfiguration()
    {
        var functionName = "configuration.get_email_configuration";
        var result = await _dataService.ExecuteQueryFirstOrDefault<Email>(functionName);
        return result;
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

    public async Task<Spine> GetSpineConfiguration()
    {
        var functionName = "configuration.get_spine_configuration";
        var result = await _dataService.ExecuteQueryFirstOrDefault<Spine>(functionName);
        return result;
    }

    public async Task<General> GetGeneralConfiguration()
    {
        var functionName = "configuration.get_general_configuration";
        var result = await _dataService.ExecuteQueryFirstOrDefault<General>(functionName);
        return result;
    }

    public async Task<SpineMessageType> GetSpineMessageType(Helpers.Constants.SpineMessageTypes spineMessageType)
    {
        var functionName = "configuration.get_spine_message_type";
        var result = await _dataService.ExecuteQuery<SpineMessageType>(functionName);
        return result.FirstOrDefault(x => x.SpineMessageTypeId == (int)spineMessageType);
    }

    public async Task<IEnumerable<SpineMessageType>> GetSpineMessageTypes()
    {
        var functionName = "configuration.get_spine_message_type";
        var result = await _dataService.ExecuteQuery<SpineMessageType>(functionName);
        return result;
    }

    public async Task<Sso> GetSsoConfiguration()
    {
        var functionName = "configuration.get_sso_configuration";
        var result = await _dataService.ExecuteQueryFirstOrDefault<Sso>(functionName);
        return result;
    }
}
