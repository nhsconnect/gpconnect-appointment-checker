using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gpconnect_appointment_checker.DAL.Logging
{
    public class LogService : ILogService
    {
        private readonly IDataService _dataService;
        private readonly IHttpContextAccessor _context;

        public LogService(IDataService dataService, IHttpContextAccessor context)
        {
            _dataService = dataService;
            _context = context;
        }

        public void AddErrorLog(DTO.Request.Logging.ErrorLog errorLog)
        {
            var functionName = "logging.log_error";
            var parameters = new DynamicParameters();
            parameters.Add("_application", errorLog.Application);
            parameters.Add("_logged", errorLog.Logged);
            parameters.Add("_level", errorLog.Level);
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")), DbType.Int32);
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
            parameters.Add("_message", errorLog.Message);
            parameters.Add("_logger", errorLog.Logger);
            parameters.Add("_callsite", errorLog.Callsite);
            parameters.Add("_exception", errorLog.Exception);
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public DTO.Response.Logging.SpineMessage AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage)
        {
            var functionName = "logging.log_spine_message";
            var parameters = new DynamicParameters();
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")), DbType.Int32);
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
            parameters.Add("_roundtriptime_ms", spineMessage.RoundTripTimeMs, DbType.Int32);
            if (spineMessage.SearchResultId > 0)
            {
                parameters.Add("_search_result_id", spineMessage.SearchResultId);
            }
            else
            {
                parameters.Add("_search_result_id", DBNull.Value, DbType.Int32);
            }

            var result = _dataService.ExecuteFunction<DTO.Response.Logging.SpineMessage>(functionName, parameters).FirstOrDefault();
            return result;
        }

        public void UpdateSpineMessageLog(int spineMessageId, int searchResultId)
        {
            var functionName = "logging.update_spine_message";
            var parameters = new DynamicParameters();
            parameters.Add("_spine_message_id", spineMessageId);
            parameters.Add("_search_result_id", searchResultId);
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest)
        {
            var functionName = "logging.log_web_request";
            var parameters = new DynamicParameters();
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")), DbType.Int32);
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
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public List<DTO.Response.Logging.PurgeErrorLog> PurgeLogEntries()
        {
            var functionName = "logging.purge_logs";
            var result = _dataService.ExecuteFunction<DTO.Response.Logging.PurgeErrorLog>(functionName);
            return result;
        }
    }
}
