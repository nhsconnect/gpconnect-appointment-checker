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
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context, ILogService logService)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                logService.AddWebRequestLog(new WebRequest
                {
                    CreatedBy = context.User?.GetClaimValue("DisplayName"),
                    Url = context.Request?.Path.Value,
                    Description = "",
                    Ip = context.Connection?.LocalIpAddress.ToString(),
                    Server = context.Request?.Host.Host,
                    SessionId = context.GetSessionId(),
                    ReferrerUrl = context.Request?.Headers["Referer"].ToString(),
                    ResponseCode = context.Response.StatusCode,
                    UserSessionId = Convert.ToInt32(context.User.GetClaimValue("UserSessionId", nullIfEmpty: true)),
                    UserId = Convert.ToInt32(context.User.GetClaimValue("UserId", nullIfEmpty: true)),
                    UserAgent = context.Request?.Headers["User-Agent"].ToString()
                });
            }
        }
    }

}
