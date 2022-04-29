using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;

namespace gpconnect_appointment_checker.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class StatusCodeModel : BaseModel
    {
        protected ILogger<StatusCodeModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public StatusCodeModel(IOptionsMonitor<General> configuration, IHttpContextAccessor contextAccessor, ILogger<StatusCodeModel> logger) : base(configuration, contextAccessor) 
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }

        public void OnGet(int statusCode)
        {
            StatusCode = statusCode;
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
            _logger.LogError($"Status code {StatusCode} thrown. Reason phrase is {ReasonPhrase}");
        }
    }
}
