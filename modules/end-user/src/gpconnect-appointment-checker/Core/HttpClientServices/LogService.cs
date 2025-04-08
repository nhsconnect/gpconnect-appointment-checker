using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        _logger = logger;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task AddWebRequest()
    {
        try
        {
            var url = _contextAccessor.HttpContext.Request?.Path.Value;            

            if (!url.Contains(SystemConstants.Healthcheckerpath))
            {
                var webRequest = new WebRequest()
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
                };

                var json = new StringContent(
                    JsonConvert.SerializeObject(webRequest, null, _options),
                    Encoding.UTF8,
                    MediaTypeHeaderValue.Parse("application/json").MediaType);

                var response = await _httpClient.PostWithHeadersAsync("/log/webrequest", new Dictionary<string, string>()
                {
                    [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
                }, json);
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An error has occurred trying to write a web request entry - {DateTime.Now} {_httpClient.BaseAddress}");
        }
    }
}
