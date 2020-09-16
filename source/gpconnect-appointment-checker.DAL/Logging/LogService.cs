using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.DAL.Logging
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly IDataService _dataService;

        public LogService(IConfiguration configuration, ILogger<LogService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async void AddErrorLog(DTO.Request.Logging.ErrorLog errorLog)
        {
            var functionName = "logging.log_error";
            var parameters = new DynamicParameters();
            parameters.Add("_application", errorLog.Application);
            parameters.Add("_logged", errorLog.Logged);
            parameters.Add("_level", errorLog.Level);
            parameters.Add("_user_id", errorLog.UserId);
            parameters.Add("_user_session_id", errorLog.UserSessionId);
            parameters.Add("_message", errorLog.Message);
            parameters.Add("_logger", errorLog.Logger);
            parameters.Add("_callsite", errorLog.Callsite);
            parameters.Add("_exception", errorLog.Exception);
            await _dataService.ExecuteFunction(functionName, parameters);
        }

        public async void AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage)
        {
            var functionName = "logging.log_spine_message";
            var parameters = new DynamicParameters();
            parameters.Add("_user_session_id", spineMessage.UserSessionId);
            parameters.Add("_spine_message_type_id", spineMessage.SpineMessageTypeId);
            parameters.Add("_command", spineMessage.Command);
            parameters.Add("_request_headers", spineMessage.RequestHeaders);
            parameters.Add("_request_payload", spineMessage.RequestPayload);
            parameters.Add("_response_status", spineMessage.ResponseStatus);
            parameters.Add("_response_headers", spineMessage.ResponseHeaders);
            parameters.Add("_response_payload", spineMessage.ResponsePayload);
            parameters.Add("_roundtriptime_ms", spineMessage.RoundTripTimeMs);
            await _dataService.ExecuteFunction(functionName, parameters);
        }

        public async void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest)
        {
            var functionName = "logging.log_web_request";
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", webRequest.UserId);
            parameters.Add("_user_session_id", webRequest.UserSessionId);
            parameters.Add("_url", webRequest.Url);
            parameters.Add("_referrer_url", webRequest.ReferrerUrl);
            parameters.Add("_description", webRequest.Description);
            parameters.Add("_ip", webRequest.Ip);
            parameters.Add("_created_date", webRequest.CreatedDate);
            parameters.Add("_created_by", webRequest.CreatedBy);
            parameters.Add("_server", webRequest.Server);
            parameters.Add("_response_code", webRequest.ResponseCode);
            parameters.Add("_session_id", webRequest.SessionId);
            parameters.Add("_user_agent", webRequest.UserAgent);
            await _dataService.ExecuteFunction(functionName, parameters);
        }
    }
}
