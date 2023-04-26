using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace GpConnect.AppointmentChecker.Api.Core.HttpClientServices;

public static class HttpClientExtensions
{
    public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        Action<HttpClient> httpClientConfig = options =>
        {
            options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        };
        services.AddHttpClient<IOrganisationService, OrganisationService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient("GpConnectClient", options =>
        {
            options.Timeout = new TimeSpan(0, 0, 0, 30);
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }).ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(env)).SetHandlerLifetime(System.Threading.Timeout.InfiniteTimeSpan);
    }

    private static IHttpClientBuilder AugmentHttpClientBuilder(this IHttpClientBuilder httpClientBuilder, IWebHostEnvironment env)
    {
        return httpClientBuilder.AddPolicyHandler(GetRetryPolicy())
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(env));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));
    }

    private static HttpMessageHandler CreateHttpMessageHandler(IWebHostEnvironment env)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
        return httpClientHandler;
    }
}
