using gpconnect_appointment_checker.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class LoggingExtensions
    {
        public static IServiceCollection ConfigureLoggingServices(this IServiceCollection services, IConfiguration configuration)
        {
            //var nLogConfiguration = new NLog.Config.LoggingConfiguration();

            //var consoleTarget = AddConsoleTarget();
            //nLogConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
            //nLogConfiguration.AddTarget(consoleTarget);

            //nLogConfiguration.Variables.Add("applicationVersion", ApplicationHelper.ApplicationVersion.GetAssemblyVersion());
            //nLogConfiguration.Variables.Add("userId", new Layout<int>(0));
            //nLogConfiguration.Variables.Add("userSessionId", new Layout<int>(0));

            //LogManager.Configuration = nLogConfiguration;

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog(configuration.GetSection("Logging"));
            });

            return services;
        }
        private static ConsoleTarget AddConsoleTarget()
        {
            var consoleTarget = new ConsoleTarget
            {
                Name = "Console",
                Layout = "${var:applicationVersion}|${date}|${level:uppercase=true}|${message}|${logger}|${callsite:filename=true}|${exception:format=stackTrace}|${var:userId}|${var:userSessionId}"
            };
            return consoleTarget;
        }
    }
}
