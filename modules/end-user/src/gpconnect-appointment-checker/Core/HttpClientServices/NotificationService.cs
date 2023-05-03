using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _serialiserOptions;
    private readonly IHttpContextAccessor _contextAccessor;

    public NotificationService(HttpClient httpClient, IHttpContextAccessor contextAccessor, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _serialiserOptions = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        _contextAccessor = contextAccessor;
    }

    public async Task PostNotificationAsync(NotificationDetails notificationDetails)
    {
        notificationDetails.TemplateParameters.Add("url", $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}");
        var content = new StringContent(JsonConvert.SerializeObject(notificationDetails, null, _serialiserOptions),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/notification", content);

        response.EnsureSuccessStatusCode();
    }
}
