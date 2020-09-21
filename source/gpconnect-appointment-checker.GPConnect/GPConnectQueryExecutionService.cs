using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.GPConnect
{
    public class GPConnectQueryExecutionService : IGPConnectQueryExecutionService
    {
        private readonly ILogger<GPConnectQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;

        public GPConnectQueryExecutionService(ILogger<GPConnectQueryExecutionService> logger, IConfigurationService configurationService, ILogService logService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
        }
    }
}
