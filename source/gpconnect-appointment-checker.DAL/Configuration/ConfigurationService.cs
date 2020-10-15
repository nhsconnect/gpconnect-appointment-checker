using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        //public async Task<DTO.Response.Configuration.General> GetGeneralConfiguration()
        //{
        //    var functionName = "configuration.get_general_configuration";
        //    var results = await _dataService.ExecuteFunction<DTO.Response.Configuration.General>(functionName);
        //    return results.FirstOrDefault();
        //}

        //public async Task<DTO.Response.Configuration.Spine> GetSpineConfiguration()
        //{
        //    var functionName = "configuration.get_spine_configuration";
        //    var result = await _dataService.ExecuteFunction<DTO.Response.Configuration.Spine>(functionName);
        //    return result.FirstOrDefault();
        //}

        public async Task<List<DTO.Response.Configuration.SpineMessageType>> GetSpineMessageTypes()
        {
            var functionName = "configuration.get_spine_message_type";
            var result = await _dataService.ExecuteFunction<DTO.Response.Configuration.SpineMessageType>(functionName);
            return result;
        }

        public async Task<List<DTO.Response.Configuration.SdsQuery>> GetSdsQueryConfiguration()
        {
            var functionName = "configuration.get_sds_queries";
            var result = await _dataService.ExecuteFunction<DTO.Response.Configuration.SdsQuery>(functionName);
            return result;
        }

        //public async Task<DTO.Response.Configuration.Sso> GetSsoConfiguration()
        //{
        //    var functionName = "configuration.get_sso_configuration";
        //    var result = await _dataService.ExecuteFunction<DTO.Response.Configuration.Sso>(functionName);
        //    return result.FirstOrDefault();
        //}
    }
}
