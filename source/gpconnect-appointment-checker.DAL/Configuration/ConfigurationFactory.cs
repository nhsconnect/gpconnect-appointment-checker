using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Configuration
{
    [Serializable]
    public class ConfigurationFactory : DataInterface
    {
        protected ILogger<ConfigurationFactory> _logger;

        public ConfigurationFactory(IConfiguration configuration, ILogger<ConfigurationFactory> logger) : base(configuration, logger)
        {
            _logger = logger;
        }

        public async Task<List<DTO.Response.Configuration.General>> GetGeneralConfigurations()
        {
            var functionName = "configuration.get_general_configuration";
            var results = await ExecuteFunction<List<DTO.Response.Configuration.General>>(functionName);
            return results;
        }

        public async Task<List<DTO.Response.Configuration.Spine>> GetSpineConfigurations()
        {
            var functionName = "configuration.get_spine_configuration";
            var results = await ExecuteFunction<List<DTO.Response.Configuration.Spine>>(functionName);
            return results;
        }
    }
}
