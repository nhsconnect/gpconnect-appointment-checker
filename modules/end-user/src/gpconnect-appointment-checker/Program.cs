using Autofac.Extensions.DependencyInjection;
using GpConnect.AppointmentChecker.Core.Configuration;
using gpconnect_appointment_checker.Configuration.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using System;

namespace gpconnect_appointment_checker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args, logger).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args, Logger logger) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(options => options.AddServerHeader = false);
                })
                .ConfigureAppConfiguration((builderContext, configurationBuilder) =>
                {
                    logger.Info("AddCustomConfiguration");
                    CustomConfigurationBuilder.AddCustomConfiguration(builderContext, configurationBuilder);
                })
                .ConfigureLogging((builderContext, logging) =>
                {
                    logger.Info("AddLoggingConfiguration");
                    LoggingConfigurationBuilder.AddLoggingConfiguration(builderContext, logging);
                })
                .UseNLog();
    }
}
