using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public static class HttpClientExtensions
{
    public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        Action<HttpClient> httpClientConfig = options =>
        {
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            options.BaseAddress = new UriBuilder(configuration.GetSection("ApiBaseUrl").Value).Uri;
        };

        services.AddHttpClient<IApplicationService, ApplicationService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<IConfigurationService, ConfigurationService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<ILogService, LogService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<INotificationService, NotificationService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<IReportingService, ReportingService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<ISpineService, SpineService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<ITokenService, TokenService>(httpClientConfig).AugmentHttpClientBuilder(env);
        services.AddHttpClient<IUserService, UserService>(httpClientConfig).AugmentHttpClientBuilder(env);        
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
