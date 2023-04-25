using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IConfigurationService
{
    Task<IEnumerable<SpineMessageType>> GetSpineMessageTypes();
    Task<SpineMessageType> GetSpineMessageType(Helpers.Constants.SpineMessageTypes spineMessageType);
    Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
    Task<SdsQuery> GetSdsQueryConfiguration(string queryName);
    Task<FhirApiQuery> GetFhirApiQueryConfiguration(string queryName);

    Task<IEnumerable<SdsQuery>> GetSdsQueryConfiguration();
    Task<IEnumerable<FhirApiQuery>> GetFhirApiQueryConfiguration();
}