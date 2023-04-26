using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.Service;

public class LogService : ILogService
{
    private readonly IDataService _dataService;

    public LogService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task AddErrorLog(DTO.Request.Logging.ErrorLog errorLog)
    {
        var functionName = "logging.log_error";
        var parameters = new DynamicParameters();
        parameters.Add("_application", errorLog.Application);
        parameters.Add("_logged", errorLog.Logged);
        parameters.Add("_level", errorLog.Level);
        if (errorLog.UserId > 0)
        {
            parameters.Add("_user_id", errorLog.UserId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_id", DBNull.Value, DbType.Int32);
        }
        if (errorLog.UserSessionId > 0)
        {
            parameters.Add("_user_session_id", errorLog.UserSessionId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_session_id", DBNull.Value, DbType.Int32);
        }
        parameters.Add("_message", errorLog.Message);
        parameters.Add("_logger", errorLog.Logger);
        parameters.Add("_callsite", errorLog.Callsite);
        parameters.Add("_exception", errorLog.Exception);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<DTO.Response.Logging.SpineMessage> AddSpineMessageLog(SpineMessage spineMessage)
    {
        var functionName = "logging.log_spine_message";
        var parameters = new DynamicParameters();
        if (spineMessage.UserSessionId > 0)
        {
            parameters.Add("_user_session_id", spineMessage.UserSessionId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_session_id", DBNull.Value, DbType.Int32);
        }
        parameters.Add("_spine_message_type_id", spineMessage.SpineMessageTypeId);
        parameters.Add("_command", spineMessage.Command);
        parameters.Add("_request_headers", spineMessage.RequestHeaders);
        parameters.Add("_request_payload", spineMessage.RequestPayload);
        parameters.Add("_response_status", spineMessage.ResponseStatus);
        parameters.Add("_response_headers", spineMessage.ResponseHeaders);
        parameters.Add("_response_payload", spineMessage.ResponsePayload ?? string.Empty);
        parameters.Add("_roundtriptime_ms", spineMessage.RoundTripTimeMs, DbType.Double);
        if (spineMessage.SearchResultId > 0)
        {
            parameters.Add("_search_result_id", spineMessage.SearchResultId);
        }
        else
        {
            parameters.Add("_search_result_id", DBNull.Value, DbType.Int32);
        }

        var result = await _dataService.ExecuteQueryFirstOrDefault<DTO.Response.Logging.SpineMessage>(functionName, parameters);
        return result;
    }

    public async Task UpdateSpineMessageLog(int spineMessageId, int searchResultId)
    {
        var functionName = "logging.update_spine_message";
        var parameters = new DynamicParameters();
        parameters.Add("_spine_message_id", spineMessageId);
        parameters.Add("_search_result_id", searchResultId);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task AddWebRequestLog(WebRequest webRequest)
    {
        var functionName = "logging.log_web_request";
        var parameters = new DynamicParameters();
        if (webRequest.UserId > 0)
        {
            parameters.Add("_user_id", webRequest.UserId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_id", DBNull.Value, DbType.Int32);
        }
        if (webRequest.UserSessionId > 0)
        {
            parameters.Add("_user_session_id", webRequest.UserSessionId, DbType.Int32);
        }
        else
        {
            parameters.Add("_user_session_id", DBNull.Value, DbType.Int32);
        }
        parameters.Add("_url", webRequest.Url);
        parameters.Add("_referrer_url", webRequest.ReferrerUrl);
        parameters.Add("_description", webRequest.Description);
        parameters.Add("_ip", webRequest.Ip);
        parameters.Add("_created_date", DateTime.UtcNow);
        parameters.Add("_created_by", webRequest.CreatedBy);
        parameters.Add("_server", webRequest.Server);
        parameters.Add("_response_code", webRequest.ResponseCode);
        parameters.Add("_session_id", webRequest.SessionId);
        parameters.Add("_user_agent", webRequest.UserAgent);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<List<DTO.Response.Logging.PurgeErrorLog>> PurgeLogEntries()
    {
        var functionName = "logging.purge_logs";
        var result = await _dataService.ExecuteQuery<DTO.Response.Logging.PurgeErrorLog>(functionName);
        return result;
    }
}
