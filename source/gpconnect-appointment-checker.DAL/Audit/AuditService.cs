using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using System;

namespace gpconnect_appointment_checker.DAL.Audit
{
    public class AuditService : IAuditService
    {
        private readonly IDataService _dataService;
        private readonly IHttpContextAccessor _context;
        private readonly int? _userId;
        private readonly int? _userSessionId;

        public AuditService(IDataService dataService, IHttpContextAccessor context)
        {
            _context = context;
            _dataService = dataService;
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
                _userId = Convert.ToInt32(_context.HttpContext.User.GetClaimValue("UserId"));
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
                _userSessionId = Convert.ToInt32(_context.HttpContext.User.GetClaimValue("UserSessionId"));
        }

        public void AddEntry(DTO.Request.Audit.Entry auditEntry)
        {
            var functionName = "audit.add_entry";
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", _userId);
            parameters.Add("_user_session_id", _userSessionId);
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
