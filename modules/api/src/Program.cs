using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using gpconnect_appointment_checker.api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Core;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Core.Mapping;
using GpConnect.AppointmentChecker.Core.Configuration;
using NLog;
using NLog.Web;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;

try
{
    var builder = WebApplication.CreateBuilder(args);

    CustomConfigurationBuilder.AddCustomConfiguration(builder.Configuration);
    // Setup Logging (commented out for now - you might want to enable it later)
    // builder.Logging.ConfigureCloudWatchLogging(builder.Configuration);

    // Register Services
    builder.Services.AddOptions();
    builder.Services.AddHttpContextAccessor();

    builder.Services.ConfigureApplicationServices(builder.Configuration, builder.Environment);
    // builder.Services.ConfigureLoggingServices(builder.Configuration);

    // Setup Host and Port based on environment
    var port = !builder.Environment.IsProduction() ? "5002" : "8080";
    var host = builder.Environment.IsProduction() ? "+" : "localhost";
    builder.WebHost.UseUrls($"http://{host}:{port}");

    // Build the app
    var app = builder.Build();

    // Configure Middleware Pipeline
    app.ConfigureApplicationBuilderServices(app.Environment);

    // Configure Mapping (TODO: remove AutoMapper)
    MappingExtensions.ConfigureMappingServices();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Test log using ILogger — app has started tests 2.");

    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, ex.Message);
    throw;
}
finally
{
    LogManager.Shutdown();
}