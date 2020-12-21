using gpconnect_appointment_checker.Configuration.Infrastructure;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString(ConnectionStrings.DefaultConnection)))
                throw new ArgumentException($"Environment variable ConnectionStrings:{ConnectionStrings.DefaultConnection} is not present");

            Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }
        public IApplicationService _applicationService { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.ConfigureAuthenticationServices(Configuration);
            services.ConfigureApplicationServices(Configuration, WebHostEnvironment);
            services.ConfigureLoggingServices(Configuration);
            MappingExtensions.ConfigureMappingServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAuditService auditService, IApplicationService applicationService)
        {
            _applicationService = applicationService;
            app.ConfigureApplicationBuilderServices(env);
        }
    }
}
