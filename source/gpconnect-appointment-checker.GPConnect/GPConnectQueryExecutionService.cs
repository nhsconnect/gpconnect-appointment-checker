using System;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.GPConnect
{
    public class GPConnectQueryExecutionService : IGPConnectQueryExecutionService
    {
        private readonly ILogger<GPConnectQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;
        private readonly IHttpClientFactory _clientFactory;

        public GPConnectQueryExecutionService(ILogger<GPConnectQueryExecutionService> logger, IConfigurationService configurationService, ILogService logService, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
            _clientFactory = clientFactory;
        }

        public async Task<T> ExecuteGet<T>(HttpRequestMessage request) where T : class
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<T>(responseStream);
                    return result;
                }
                return null;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error occurred in trying to execute a GET request", exc);
                throw;
            }
        }
    }
}
