using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.DAL.Logging
{
    public class LogService : ILogService
    {
        private readonly IDataService _dataService;
        private readonly IHttpContextAccessor _context;
        private readonly int? _userId;
        private readonly int? _userSessionId;

        public LogService(IDataService dataService, IHttpContextAccessor context)
        {
            _dataService = dataService;
            _context = context;
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null) 
                _userId = Convert.ToInt32(_context.HttpContext.User.GetClaimValue("UserId"));
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
                _userSessionId = Convert.ToInt32(_context.HttpContext.User.GetClaimValue("UserSessionId"));
        }

        public void AddErrorLog(DTO.Request.Logging.ErrorLog errorLog)
        {
            var functionName = "logging.log_error";
            var parameters = new DynamicParameters();
            parameters.Add("_application", errorLog.Application);
            parameters.Add("_logged", errorLog.Logged);
            parameters.Add("_level", errorLog.Level);
            parameters.Add("_user_id", _userId);
            parameters.Add("_user_session_id", _userSessionId);
            parameters.Add("_message", errorLog.Message);
            parameters.Add("_logger", errorLog.Logger);
            parameters.Add("_callsite", errorLog.Callsite);
            parameters.Add("_exception", errorLog.Exception);
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public void AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage)
        {
            var functionName = "logging.log_spine_message";
            var parameters = new DynamicParameters();
            parameters.Add("_user_session_id", _userSessionId);
            parameters.Add("_spine_message_type_id", spineMessage.SpineMessageTypeId);
            parameters.Add("_command", spineMessage.Command);
            parameters.Add("_request_headers", spineMessage.RequestHeaders);
            parameters.Add("_request_payload", spineMessage.RequestPayload);
            parameters.Add("_response_status", spineMessage.ResponseStatus);
            parameters.Add("_response_headers", spineMessage.ResponseHeaders);
            parameters.Add("_response_payload", spineMessage.ResponsePayload ?? string.Empty);
            parameters.Add("_roundtriptime_ms", spineMessage.RoundTripTimeMs, DbType.Int32);
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest)
        {
            var functionName = "logging.log_web_request";
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", _userId);
            parameters.Add("_user_session_id", _userSessionId);
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
