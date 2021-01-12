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
            var parameters = new DynamicParameters();
            parameters.Add("_application", errorLog.Application);
            parameters.Add("_logged", errorLog.Logged);
            parameters.Add("_level", errorLog.Level);
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")), DbType.Int32, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_user_id", DBNull.Value, DbType.Int32, ParameterDirection.Input);
            }
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")), DbType.Int32, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_user_session_id", DBNull.Value, DbType.Int32, ParameterDirection.Input);
            }
            parameters.Add("_message", errorLog.Message, DbType.String, ParameterDirection.Input);
            parameters.Add("_logger", errorLog.Logger, DbType.String, ParameterDirection.Input);
            parameters.Add("_callsite", errorLog.Callsite, DbType.String, ParameterDirection.Input);
            parameters.Add("_exception", errorLog.Exception, DbType.String, ParameterDirection.Input);
            _dataService.ExecuteFunction(Constants.Schemas.Logging, Constants.Functions.LogError, parameters);
        }

        public void AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage)
        {
            var parameters = new DynamicParameters();
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")), DbType.Int32, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_user_session_id", DBNull.Value, DbType.Int32, ParameterDirection.Input);
            }
            parameters.Add("_spine_message_type_id", spineMessage.SpineMessageTypeId);
            parameters.Add("_command", spineMessage.Command, DbType.String, ParameterDirection.Input);
            parameters.Add("_request_headers", spineMessage.RequestHeaders, DbType.String, ParameterDirection.Input);
            parameters.Add("_request_payload", spineMessage.RequestPayload, DbType.String, ParameterDirection.Input);
            parameters.Add("_response_status", spineMessage.ResponseStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("_response_headers", spineMessage.ResponseHeaders, DbType.String, ParameterDirection.Input);
            parameters.Add("_response_payload", spineMessage.ResponsePayload ?? string.Empty, DbType.String, ParameterDirection.Input);
            parameters.Add("_roundtriptime_ms", spineMessage.RoundTripTimeMs, DbType.Int32, ParameterDirection.Input);
            _dataService.ExecuteFunction(Constants.Schemas.Logging, Constants.Functions.LogSpineMessage, parameters);
        }

        public void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest)
        {
            var parameters = new DynamicParameters();
            if (_context.HttpContext?.User?.GetClaimValue("UserId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")), DbType.Int32, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_user_id", DBNull.Value, DbType.Int32, ParameterDirection.Input);
            }
            if (_context.HttpContext?.User?.GetClaimValue("UserSessionId", nullIfEmpty: true) != null)
            {
                parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")), DbType.Int32, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_user_session_id", DBNull.Value, DbType.Int32, ParameterDirection.Input);
            }
            parameters.Add("_url", webRequest.Url, DbType.String, ParameterDirection.Input);
            parameters.Add("_referrer_url", webRequest.ReferrerUrl, DbType.String, ParameterDirection.Input);
            parameters.Add("_description", webRequest.Description, DbType.String, ParameterDirection.Input);
            parameters.Add("_ip", webRequest.Ip, DbType.String, ParameterDirection.Input);
            parameters.Add("_created_date", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("_created_by", webRequest.CreatedBy, DbType.String, ParameterDirection.Input);
            parameters.Add("_server", webRequest.Server, DbType.String, ParameterDirection.Input);
            parameters.Add("_response_code", webRequest.ResponseCode, DbType.Int32, ParameterDirection.Input);
            parameters.Add("_session_id", webRequest.SessionId, DbType.String, ParameterDirection.Input);
            parameters.Add("_user_agent", webRequest.UserAgent, DbType.String, ParameterDirection.Input);
            _dataService.ExecuteFunction(Constants.Schemas.Logging, Constants.Functions.LogWebRequest, parameters);
        }

        public List<DTO.Response.Logging.PurgeErrorLog> PurgeLogEntries()
        {
            var result = _dataService.ExecuteFunction<DTO.Response.Logging.PurgeErrorLog>(Constants.Schemas.Logging, Constants.Functions.PurgeLogs);
            return result;
        }
    }
}
