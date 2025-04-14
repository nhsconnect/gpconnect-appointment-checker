namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public static class CustomConfigurationBuilder
{
    public static void AddCustomConfiguration(ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}