using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using System;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public static class CustomConfigurationBuilder
{
    public static void AddCustomConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);        

        if (!context.HostingEnvironment.IsDevelopment())
        {
            logger.Info("Not IsDevelopment");
            logger.Info("START builder.AddAmazonSecretsManager");
            builder.AddAmazonSecretsManager();
            logger.Info("END builder.AddAmazonSecretsManager");
        }
        else
        {
            logger.Info("Is Development");
        }        

        builder.AddEnvironmentVariables();
    }

    public static void AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder)
    {
        var configurationSource = new AmazonSecretsManagerConfigurationSource();
        configurationBuilder.Add(configurationSource);
    }
}
