using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.DAL.Audit
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;
        private readonly IDataService _dataService;

        public AuditService(IConfiguration configuration, ILogger<AuditService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public void AddEntry(DTO.Request.Audit.Entry auditEntry)
        {
            var functionName = "audit.add_entry";
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", auditEntry.UserId);
            parameters.Add("_user_session_id", auditEntry.UserSessionId);
            parameters.Add("_entry_type_id", auditEntry.EntryTypeId);
            parameters.Add("_item1", auditEntry.Item1);
            parameters.Add("_item2", auditEntry.Item2);
            parameters.Add("_item3", auditEntry.Item3);
            parameters.Add("_details", auditEntry.Details);
            parameters.Add("_entry_elapsed_ms", auditEntry.EntryElapsedMs);
            _dataService.ExecuteFunction(functionName, parameters);
        }
    }
}
