using GpConnect.AppointmentChecker.Api.Core;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Core.Mapping;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);

    // Load Custom Configuration
    CustomConfigurationBuilder.AddCustomConfiguration(builder.Configuration, builder.Environment);

    // Setup Logging
    LoggingConfigurationBuilder.AddLoggingConfiguration(builder.Logging, builder.Configuration);

    builder.Host.UseNLog();

    // Register Services
    builder.Services.AddOptions();
    builder.Services.AddHttpContextAccessor();

    builder.Services.ConfigureApplicationServices(builder.Configuration, builder.Environment);
    builder.Services.ConfigureLoggingServices(builder.Configuration);

    var port = !builder.Environment.IsProduction() ? "5002" : "8080";
    var host = builder.Environment.IsProduction() ? "+" : "localhost";
    builder.WebHost.UseUrls($"http://{host}:{port}");

    builder.Logging.AddConsole();

    // Build the app
    var app = builder.Build();

    // Configure Middleware Pipeline
    app.ConfigureApplicationBuilderServices(app.Environment);


    //TODO: Remove AutoMapper - AutoMapper is the devil
    MappingExtensions.ConfigureMappingServices();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, ex.Message);
    throw;
}
finally
{
    LogManager.Shutdown();
}