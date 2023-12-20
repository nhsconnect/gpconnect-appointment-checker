using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class LoggingExtensions
    {
        public static IServiceCollection ConfigureLoggingServices(this IServiceCollection services, IConfiguration configuration)
        {
            var nLogConfiguration = new NLog.Config.LoggingConfiguration();

            nLogConfiguration.Variables.Add(Helpers.Constants.Headers.UserId, null);

            LogManager.Configuration = nLogConfiguration;

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(configuration.GetSection("Logging"));
            });

            return services;
        }
    }
}
