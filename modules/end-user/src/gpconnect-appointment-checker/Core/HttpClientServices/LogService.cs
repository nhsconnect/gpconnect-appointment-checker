using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class LogService : ILogService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<LogService> _logger;

    public LogService(HttpClient httpClient, IHttpContextAccessor contextAccessor, ILogger<LogService> logger, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _contextAccessor = contextAccessor;
        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _logger = logger;
    }

    public async Task AddWebRequest()
    {
        try
        {
            var url = _contextAccessor.HttpContext.Request?.Path.Value;
            if (!url.Contains(gpconnect_appointment_checker.Helpers.Constants.SystemConstants.HEALTHCHECKERPATH))
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new Models.Request.WebRequest()
                    {
                        CreatedBy = _contextAccessor.HttpContext.User?.GetClaimValue("DisplayName"),
                        Url = url,
                        Ip = _contextAccessor.HttpContext.Connection?.LocalIpAddress.ToString(),
                        Description = string.Empty,
                        Server = _contextAccessor.HttpContext.Request?.Host.Host,
                        SessionId = _contextAccessor.HttpContext.GetSessionId(),
                        ReferrerUrl = _contextAccessor.HttpContext.Request?.Headers["Referrer"].ToString(),
                        ResponseCode = _contextAccessor.HttpContext.Response.StatusCode,
                        UserAgent = _contextAccessor.HttpContext.Request?.Headers["User-Agent"].ToString()
                    }));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                //var response = await _httpClient.PostAsync("log/webrequest", content);
                //response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An error has occurred trying to write a web request entry - {_httpClient.BaseAddress}");
        }
    }
}
