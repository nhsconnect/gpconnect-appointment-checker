using GpConnect.AppointmentChecker.Api.Core;
using GpConnect.AppointmentChecker.Api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Core.Mapping;

namespace GpConnect.AppointmentChecker.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddHttpContextAccessor();

        services.ConfigureApplicationServices(_configuration);
        services.ConfigureLoggingServices(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.ConfigureApplicationBuilderServices(env);
        MappingExtensions.ConfigureMappingServices();
    }
}