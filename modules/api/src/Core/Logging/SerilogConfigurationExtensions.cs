using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;

namespace gpconnect_appointment_checker.api.Core.Logging
{
    public static class LoggingExtensions
    {
        public static void ConfigureCloudWatchLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            var logGroup = configuration.GetValue<string>("Logging:LogGroupName");
            var logStreamPrefix = configuration.GetValue<string>("Logging:LogStreamPrefix");
            
            var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

            // use ECS in dev / production & local aws stored profile for local 
            IAmazonCloudWatchLogs client = isProduction
                ? new AmazonCloudWatchLogsClient(Amazon.RegionEndpoint.EUWest2)
                : new AmazonCloudWatchLogsClient(new StoredProfileAWSCredentials("def"), Amazon.RegionEndpoint.EUWest2);

            try
            {
                // Setup Serilog
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console() // Always log to console for local dev (debugging)
                    .WriteTo.AmazonCloudWatch(
                        logGroup: logGroup,
                        logStreamPrefix: logStreamPrefix,
                        cloudWatchClient: client,
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                        textFormatter: new JsonFormatter()) // Logs to CloudWatch in production
                    .CreateLogger();

                // Add Serilog as the logging provider to the ILoggerFactory
                loggingBuilder.AddSerilog();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AWS SDK or Serilog setup: {ex.Message}");
            }
        }
    }
}