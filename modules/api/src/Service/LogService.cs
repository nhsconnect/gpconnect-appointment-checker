using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Data;
using SpineMessage = GpConnect.AppointmentChecker.Api.DTO.Request.Logging.SpineMessage;

namespace GpConnect.AppointmentChecker.Api.Service;

public class LogService : ILogService
{
    private readonly IDataService _dataService;

    public LogService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<DTO.Response.Logging.SpineMessage> GetSpineMessageLogBySearchResultId(int searchResultId)
    {
        var functionName = "logging.get_spine_message_by_search_result_id";
        var parameters = new DynamicParameters();
        parameters.Add("_search_result_id", searchResultId, DbType.Int32);

        var result = await _dataService.ExecuteQueryFirstOrDefault<DTO.Response.Logging.SpineMessage>(functionName, parameters);
        return result;
    }

    public async Task<List<DTO.Response.Logging.SpineMessage>> GetSpineMessageLogBySearchGroupId(int searchGroupId)
    {
        var functionName = "logging.get_spine_message_by_search_group_id";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId, DbType.Int32);
        var result = await _dataService.ExecuteQuery<DTO.Response.Logging.SpineMessage>(functionName, parameters);
        return result;
    }

    public async Task AddErrorLog(ErrorLog errorLog)
    {
        var functionName = "logging.log_error";
        var parameters = new DynamicParameters();
        parameters.Add("_application", errorLog.Application);
        parameters.Add("_logged", DateTime.UtcNow);
        parameters.Add("_level", errorLog.Level, DbType.String);
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId), DbType.Int32);
        parameters.Add("_message", errorLog.Message, DbType.String);
        parameters.Add("_logger", errorLog.Logger, DbType.String);
        parameters.Add("_callsite", errorLog.Callsite, DbType.String);
        parameters.Add("_exception", errorLog.Exception, DbType.String);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<DTO.Response.Logging.SpineMessage> AddSpineMessageLog(SpineMessage spineMessage)
    {
        var functionName = "logging.log_spine_message";
        var parameters = new DynamicParameters();
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId), DbType.Int32);
        parameters.Add("_spine_message_type_id", spineMessage.SpineMessageTypeId, DbType.Int32);
        parameters.Add("_command", spineMessage.Command, DbType.String);
        parameters.Add("_request_headers", spineMessage.RequestHeaders, DbType.String);
        parameters.Add("_request_payload", spineMessage.RequestPayload, DbType.String);
        parameters.Add("_response_status", spineMessage.ResponseStatus, DbType.String);
        parameters.Add("_response_headers", spineMessage.ResponseHeaders, DbType.String);
        parameters.Add("_response_payload", spineMessage.ResponsePayload ?? string.Empty, DbType.String);
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
        if (LoggingHelper.GetIntegerValue(Headers.UserId) > 0)
        {
            parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId), DbType.Int32);
        }
        else
        {
            parameters.Add("_user_id", DBNull.Value, DbType.Int32);
        }
        parameters.Add("_url", webRequest.Url, DbType.String);
        parameters.Add("_referrer_url", webRequest.ReferrerUrl, DbType.String);
        parameters.Add("_description", webRequest.Description, DbType.String);
        parameters.Add("_ip", webRequest.Ip, DbType.String);
        parameters.Add("_created_date", DateTime.UtcNow);
        parameters.Add("_created_by", webRequest.CreatedBy, DbType.String);
        parameters.Add("_server", webRequest.Server, DbType.String);
        parameters.Add("_response_code", webRequest.ResponseCode, DbType.Int32);
        parameters.Add("_session_id", webRequest.SessionId, DbType.String);
        parameters.Add("_user_agent", webRequest.UserAgent, DbType.String);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<List<DTO.Response.Logging.PurgeErrorLog>> PurgeLogEntries()
    {
        var functionName = "logging.purge_logs";
        var result = await _dataService.ExecuteQuery<DTO.Response.Logging.PurgeErrorLog>(functionName);
        return result;
    }
}
