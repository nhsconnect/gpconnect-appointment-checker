using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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

    public LogService(HttpClient httpClient, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task AddWebRequest()
    {
        var url = _contextAccessor.HttpContext.Request?.Path.Value;
        if (!url.Contains(gpconnect_appointment_checker.Helpers.Constants.SystemConstants.HEALTHCHECKERPATH))
        {
            var webRequest = new Models.Request.WebRequest()
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

            var response = await _httpClient.PostAsync("/log/webrequest", json);
            response.EnsureSuccessStatusCode();
        }        
    }
}
