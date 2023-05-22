namespace GpConnect.AppointmentChecker.Api.Core.Logging;

public static class LoggingConfigurationBuilder
{
    public static void AddLoggingConfiguration(HostBuilderContext builderContext, ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
    }
}