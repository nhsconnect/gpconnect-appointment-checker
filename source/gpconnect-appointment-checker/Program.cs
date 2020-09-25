using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web;

namespace gpconnect_appointment_checker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exc)
            {
                logger.Error(exc, "An error occurred in the application startup process");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "GPCONNECTAPPOINTMENTCHECKER_");
                }).ConfigureLogging((builderContext, logging) => {
                    logging.ClearProviders();
                    logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                }).UseNLog();
    }
}
