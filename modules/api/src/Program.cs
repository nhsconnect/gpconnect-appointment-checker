using Serilog;
using Serilog.Formatting.Json;
using Amazon.CloudWatchLogs;
using GpConnect.AppointmentChecker.Api.Core;
using Serilog.Sinks.AwsCloudWatch;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine("[Serilog SelfLog] " + msg));

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder();

    // Add configuration files
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();


    // setup Serilog as sole log provider
#if DEBUG
    builder.Host.UseSerilog((context, services, loggerConfig) =>
    {
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
    });
#else
    builder.Host.UseSerilog((context, services, loggerConfig) =>
    {
        loggerConfig
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .WriteTo.AmazonCloudWatch(
                logGroup: context.Configuration["Logging:LogGroupName"] ?? "/ecs/gpcac-application",
                logStreamPrefix: context.Configuration["Logging:LogStreamPrefix"] ?? "gpcac-api-serilog",
                cloudWatchClient: new AmazonCloudWatchLogsClient(Amazon.RegionEndpoint.EUWest2),
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                textFormatter: new JsonFormatter());
    });
#endif


    // Add services
    builder.Services.AddAWSService<IAmazonCloudWatchLogs>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddOptions();
    builder.Services.ConfigureApplicationServices(builder.Configuration, builder.Environment);

    var port = builder.Environment.IsProduction() ? "8080" : "5002";
    var host = builder.Environment.IsProduction() ? "+" : "localhost";
    builder.WebHost.UseUrls($"http://{host}:{port}");

    var app = builder.Build();
    Log.Information("Direct Serilog log — console sink test.");

    app.ConfigureApplicationBuilderServices(app.Environment);

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Test log using ILogger — app has started tests 2.");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed!");
}
finally
{
    // Flush logs and cleanly close logger on shutdown
    Log.CloseAndFlush();
}