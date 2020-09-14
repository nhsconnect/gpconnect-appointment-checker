using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace gpconnect_appointment_checker.DAL.Audit
{
    [Serializable]
    public class AuditFactory : DataInterface
    {
        protected ILogger<AuditFactory> _logger;

        public AuditFactory(IConfiguration configuration, ILogger<AuditFactory> logger) : base(configuration, logger)
        {
            _logger = logger;
        }

        public async void AddEntry(DTO.Request.Audit.Entry auditEntry)
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
            await ExecuteFunction(functionName, parameters);
        }
    }
}
