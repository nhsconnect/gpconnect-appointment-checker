using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogService _logService;

        public RequestLoggingMiddleware(RequestDelegate next, ILogService logService)
        {
            _next = next;
            _logService = logService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                await _logService.AddWebRequest();
            }
        }
    }
}
