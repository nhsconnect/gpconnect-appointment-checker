using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service;

public class AuditService : IAuditService
{
    private readonly IDataService _dataService;

    public AuditService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task AddEntry(DTO.Request.Audit.Entry auditEntry)
    {
        var functionName = "audit.add_entry";
        var parameters = new DynamicParameters();        
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId), DbType.Int32);
        parameters.Add("_entry_type_id", auditEntry.EntryTypeId);
        parameters.Add("_item1", auditEntry.Item1);
        parameters.Add("_item2", auditEntry.Item2);
        parameters.Add("_item3", auditEntry.Item3);
        parameters.Add("_details", auditEntry.Details);
        parameters.Add("_entry_elapsed_ms", auditEntry.EntryElapsedMs);
        await _dataService.ExecuteQuery(functionName, parameters);
    }
}