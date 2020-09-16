using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gpconnect_appointment_checker.DAL.Interfaces;
using System.Linq;

namespace gpconnect_appointment_checker.DAL.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly IDataService _dataService;

        public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<List<DTO.Response.Configuration.General>> GetGeneralConfigurations()
        {
            var functionName = "configuration.get_general_configuration";
            var results = await _dataService.ExecuteFunction<DTO.Response.Configuration.General>(functionName);
            return results;
        }

        public async Task<DTO.Response.Configuration.Spine> GetSpineConfiguration()
        {
            var functionName = "configuration.get_spine_configuration";
            var result = await _dataService.ExecuteFunction<DTO.Response.Configuration.Spine>(functionName);
            return result.FirstOrDefault();
        }
    }
}
