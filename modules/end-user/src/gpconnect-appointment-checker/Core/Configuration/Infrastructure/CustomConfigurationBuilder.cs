using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public static class CustomConfigurationBuilder
{
    public static void AddCustomConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
        builder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
        builder.AddEnvironmentVariables();
    }
}
