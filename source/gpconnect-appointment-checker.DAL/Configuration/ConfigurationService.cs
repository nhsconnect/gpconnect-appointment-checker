using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.DAL.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration, IDataService dataService)
        {
            _dataService = dataService;
            _configuration = configuration;
        }

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dataService = new DataService(_configuration, null);
        }

        public List<DTO.Response.Configuration.SpineMessageType> GetSpineMessageTypes()
        {
            var functionName = "configuration.get_spine_message_type";
            var result = _dataService.ExecuteFunction<DTO.Response.Configuration.SpineMessageType>(functionName);
            return result;
        }

        public DTO.Response.Configuration.SdsQuery GetSdsQueryConfiguration(string queryName)
        {
            var functionName = "configuration.get_sds_queries";
            var result = _dataService.ExecuteFunction<DTO.Response.Configuration.SdsQuery>(functionName);
            return result.FirstOrDefault(x => x.QueryName == queryName);
        }

        public DTO.Response.Configuration.FhirApiQuery GetFhirApiQueryConfiguration(string queryName)
        {
            var functionName = "configuration.get_fhir_api_queries";
            var result = _dataService.ExecuteFunction<DTO.Response.Configuration.FhirApiQuery>(functionName);
            return result.FirstOrDefault(x => x.QueryName == queryName);
        }

        public List<DTO.Response.Configuration.OrganisationType> GetOrganisationTypes()
        {
            var functionName = "configuration.get_organisation_type";
            var result = _dataService.ExecuteFunction<DTO.Response.Configuration.OrganisationType>(functionName);
            return result;
        }
    }
}
