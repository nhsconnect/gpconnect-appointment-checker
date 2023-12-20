using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Configuration.Infrastructure;

public static class LoggingConfigurationBuilder
{
  public static void AddLoggingConfiguration(HostBuilderContext builderContext, ILoggingBuilder logging)
  {
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Information);
    logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
  }
}
