using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
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
                    UserAgent = context.Request?.Headers["User-Agent"].ToString()
                });
            }
        }
    }

}
