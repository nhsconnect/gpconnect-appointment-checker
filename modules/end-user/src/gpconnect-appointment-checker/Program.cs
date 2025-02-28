using gpconnect_appointment_checker.Configuration.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using System;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Debug("Starting application...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseNLog();
    builder.Host.ConfigureAppConfiguration(CustomConfigurationBuilder.AddCustomConfiguration);

    var configuration = builder.Configuration;
    var environment = builder.Environment;

    var services = builder.Services;
    services.AddOptions();
    services.AddHttpContextAccessor();

    var authenticationExtensions = new AuthenticationExtensions(configuration);
    authenticationExtensions.ConfigureAuthenticationServices(services);

    services.ConfigureLoggingServices(configuration);
    services.ConfigureApplicationServices(configuration, environment);
    services.AddScoped<ITokenService, TokenService>();

    var app = builder.Build();

    // Configure app services
    app.ConfigureApplicationBuilderServices(environment);

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Application startup failed");
    throw;
}
finally
{
    LogManager.Shutdown();
}