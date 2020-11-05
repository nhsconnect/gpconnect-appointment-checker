using gpconnect_appointment_checker.Configuration.Infrastructure;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next/*, ILoggerFactory loggerFactory*/)
        {
            _next = next;
            //_logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context, ILogService logService)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                var userSessionId = Convert.ToInt32(context.User.GetClaimValue("UserSessionId", nullIfEmpty: true));
                var userId = Convert.ToInt32(context.User.GetClaimValue("UserId", nullIfEmpty: true));

                logService.AddWebRequestLog(new WebRequest
                {
                    CreatedBy = context.User?.GetClaimValue("DisplayName"),
                    Url = context.Request?.Path.Value,
                    Description = "",
                    Ip = "127.0.0.1",//context.Connection?.LocalIpAddress.ToString(),
                    Server = context.Request?.Host.Host,
                    SessionId = context.GetSessionId(),
                    ReferrerUrl = context.Request?.Headers["Referer"].ToString(),
                    ResponseCode = context.Response.StatusCode,
                    UserSessionId = userSessionId,
                    UserId = userId,
                    UserAgent = context.Request?.Headers["User-Agent"].ToString()
                });
            }
        }
    }

}
