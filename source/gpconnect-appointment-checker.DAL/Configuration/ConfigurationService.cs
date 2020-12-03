using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        public List<DTO.Response.Configuration.SdsQuery> GetSdsQueryConfiguration()
        {
            var functionName = "configuration.get_sds_queries";
            var result = _dataService.ExecuteFunction<DTO.Response.Configuration.SdsQuery>(functionName);
            return result;
        }
    }
}
