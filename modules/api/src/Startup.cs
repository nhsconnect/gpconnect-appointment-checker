using GpConnect.AppointmentChecker.Api.Core;
using GpConnect.AppointmentChecker.Api.Core.Logging;
using GpConnect.AppointmentChecker.Api.Core.Mapping;

namespace GpConnect.AppointmentChecker.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddHttpContextAccessor();

        services.ConfigureApplicationServices(_configuration, _environment);
        services.ConfigureLoggingServices(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.ConfigureApplicationBuilderServices(env);
        MappingExtensions.ConfigureMappingServices();
    }
}