using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web;
using System;
using System.Reflection;
using NLog;
using NLog.Extensions.Logging;

namespace gpconnect_appointment_checker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((builderContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                        logging.AddNLog(GetLoggingConfiguration());
                    }
                )
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables();
                });

        public static LoggingConfiguration GetLoggingConfiguration()
        {

            return new LoggingConfiguration();
        }
    }
}
