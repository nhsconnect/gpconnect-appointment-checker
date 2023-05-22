using NLog.Extensions.Logging;

namespace GpConnect.AppointmentChecker.Api.Core.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection ConfigureLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddNLog(configuration.GetSection("Logging"));
        });

        return services;
    }
}