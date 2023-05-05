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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(options => options.AddServerHeader = false);
                }).ConfigureAppConfiguration(CustomConfigurationBuilder.AddCustomConfiguration)
                .ConfigureLogging((builderContext, logging) =>
                {
                    LoggingConfigurationBuilder.AddLoggingConfiguration(builderContext, logging);
                })
                .UseNLog();
    }
}
