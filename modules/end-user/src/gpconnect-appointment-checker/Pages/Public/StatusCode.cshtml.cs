using GpConnect.AppointmentChecker.Core.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace gpconnect_appointment_checker.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class StatusCodeModel : BaseModel
    {
        protected ILogger<StatusCodeModel> _logger;

        public StatusCodeModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor, ILogger<StatusCodeModel> logger) : base(configuration, contextAccessor) 
        {
            _logger = logger;
        }

        public IActionResult OnGet(int statusCode = 404)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            OriginalPath = feature?.OriginalPath;

            ResponseStatusCode = statusCode;
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);

            var stringBuilder = new StringBuilder();

            if (ResponseStatusCode.GetValueOrDefault() > 0) stringBuilder.AppendLine($"Status code {ResponseStatusCode} thrown");
            if (!string.IsNullOrEmpty(ReasonPhrase)) stringBuilder.AppendLine($"Reason phrase is {ReasonPhrase}");
            if (!string.IsNullOrEmpty(OriginalPath)) stringBuilder.AppendLine($"Path is {OriginalPath}");

            _logger.LogError(stringBuilder.ToString());

            switch (statusCode)
            {
                case 404:
                    ResponseStatusCodePage = "404";
                    break;
                default:
                    ResponseStatusCodePage = "Non404";
                    break;
            }

            return Page();
        }
    }
}
