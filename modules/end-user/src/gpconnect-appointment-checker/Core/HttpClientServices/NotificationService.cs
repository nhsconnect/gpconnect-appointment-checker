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
    private readonly HttpClient _client;
    private readonly JsonSerializerSettings _serialiserOptions;
    private readonly IHttpContextAccessor _contextAccessor;

    public NotificationService(HttpClient client, IOptions<NotificationServiceConfig> options, IHttpContextAccessor contextAccessor)
    {
        _client = client;
        _client.BaseAddress = new UriBuilder(options.Value.BaseUrl).Uri;
        _serialiserOptions = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        _contextAccessor = contextAccessor;
    }

    public async Task PostNotificationAsync(NotificationDetails notificationDetails)
    {
        notificationDetails.TemplateParameters.Add("url", $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}");
        var content = new StringContent(JsonConvert.SerializeObject(notificationDetails, null, _serialiserOptions),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _client.PostAsync("/notification", content);

        response.EnsureSuccessStatusCode();
    }

    public class NotificationServiceConfig
    {
        public string BaseUrl { get; set; } = "";
    }
}
