using System.Collections.Generic;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IConfigurationService
    {
        List<DTO.Response.Configuration.SpineMessageType> GetSpineMessageTypes();
        List<DTO.Response.Configuration.OrganisationType> GetOrganisationTypes();
        DTO.Response.Configuration.SdsQuery GetSdsQueryConfiguration(string queryName);
        DTO.Response.Configuration.FhirApiQuery GetFhirApiQueryConfiguration(string queryName);
    }
}
