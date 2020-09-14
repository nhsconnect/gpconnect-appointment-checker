using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.DTO.Response.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.Logging
{
    public class WebRequestLogger
    {
        private IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment _env;

        public WebRequestLogger(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public void LogWebRequest(WebRequest webRequest)
        {

        }
    }
}
