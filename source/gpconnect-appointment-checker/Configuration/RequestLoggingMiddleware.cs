using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        //private readonly ILogService _logService;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory/*, ILogService logService*/)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
            //_logService = logService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                //_logService.AddWebRequestLog(new WebRequest
                //{
                //    CreatedBy = context.User.GetClaimValue("DisplayName"),
                //    CreatedDate = DateTime.UtcNow,
                //    Description = "",
                //    Ip = context.Connection.LocalIpAddress.ToString(),
                //    ReferrerUrl = context.Request.Headers["Referer"].ToString(),
                //    ResponseCode = context.Response.StatusCode,
                //    Server = context.Request.Host.Host,
                //    SessionId = context.Session.Id,
                //    Url = context.Request?.Path.Value,
                //    UserSessionId = context.User.GetClaimValue("UserSessionId").StringToInteger(0),
                //    UserId = context.User.GetClaimValue("UserId").StringToInteger(0),
                //    UserAgent = context.Request.Headers["User-Agent"].ToString()
                //});
            }
        }
    }

}
