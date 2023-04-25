using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service;

public class AuditService : IAuditService
{
    private readonly IDataService _dataService;
    private readonly IHttpContextAccessor _context;

    public AuditService(IDataService dataService, IHttpContextAccessor context)
    {
        _context = context;
        _dataService = dataService;
    }

    public async Task AddEntry(DTO.Request.Audit.Entry auditEntry)
    {
        var functionName = "audit.add_entry";
        var parameters = new DynamicParameters();
        if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
        {
            parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")), DbType.Int32);
        }
        else if(auditEntry.UserId > 0)
        {
            parameters.Add("_user_id", auditEntry.UserId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_id", DBNull.Value, DbType.Int32);
        }
        if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
        {
            parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")), DbType.Int32);
        }
        else
        {
            parameters.Add("_user_session_id", DBNull.Value, DbType.Int32);
        }
        parameters.Add("_entry_type_id", auditEntry.EntryTypeId);
        parameters.Add("_item1", auditEntry.Item1);
        parameters.Add("_item2", auditEntry.Item2);
        parameters.Add("_item3", auditEntry.Item3);
        parameters.Add("_details", auditEntry.Details);
        parameters.Add("_entry_elapsed_ms", auditEntry.EntryElapsedMs);
        await _dataService.ExecuteQuery(functionName, parameters);
    }
}
