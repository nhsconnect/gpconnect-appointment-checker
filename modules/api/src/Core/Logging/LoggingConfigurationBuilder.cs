namespace GpConnect.AppointmentChecker.Api.Core.Logging;

public static class LoggingConfigurationBuilder
{
    public static void AddLoggingConfiguration(ILoggingBuilder logging, IConfiguration configuration)
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddConfiguration(configuration.GetSection("Logging"));
    }
}