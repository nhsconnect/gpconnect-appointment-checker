using GpConnect.AppointmentChecker.Api.Core;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Dal.Configuration;
using NLog;
using NLog.Web;

namespace GpConnect.AppointmentChecker.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            logger.Debug("init main");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHost((webHostBuilder) =>
            {
                WebConfigurationBuilder.ConfigureWebHost(webHostBuilder);
            })
            .UseServiceProviderFactory(new DefaultServiceProviderFactory())
            .ConfigureWebHostDefaults(webHostDefaultsBuilder =>
            {
                webHostDefaultsBuilder.UseStartup<Startup>();
                WebConfigurationBuilder.ConfigureWebHostDefaults(webHostDefaultsBuilder);
            })
            .ConfigureAppConfiguration(CustomConfigurationBuilder.AddCustomConfiguration)
            .ConfigureLogging((builderContext, logging) =>
            {
                LoggingConfigurationBuilder.AddLoggingConfiguration(builderContext, logging);
            }).UseNLog();    
}
