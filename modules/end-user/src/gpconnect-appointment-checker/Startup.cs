using Autofac;
using gpconnect_appointment_checker.Configuration.Infrastructure;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
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

            Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(); 
            services.AddHttpContextAccessor();

            services.ConfigureApplicationServices(Configuration, WebHostEnvironment);
            services.ConfigureLoggingServices(Configuration);

            var authenticationExtensions = new AuthenticationExtensions(Configuration);
            authenticationExtensions.ConfigureAuthenticationServices(services);            
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ContainerModule());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureApplicationBuilderServices(env);
        }
    }
}
