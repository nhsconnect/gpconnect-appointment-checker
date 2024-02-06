using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IConfigurationService
{
    Task<IEnumerable<SpineMessageType>> GetSpineMessageTypes();
    Task<SpineMessageType> GetSpineMessageType(Helpers.Constants.SpineMessageTypes spineMessageType, string? interactionId = null);
    Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
    Task<OrganisationType> GetOrganisationType(string organisationTypeCode);
    Task<SdsQuery> GetSdsQueryConfiguration(string queryName);
    Task<FhirApiQuery> GetFhirApiQueryConfiguration(string queryName);

    Task<IEnumerable<SdsQuery>> GetSdsQueryConfiguration();
    Task<IEnumerable<FhirApiQuery>> GetFhirApiQueryConfiguration();
}
