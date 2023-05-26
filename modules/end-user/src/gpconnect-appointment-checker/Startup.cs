using Autofac;
using gpconnect_appointment_checker.Configuration.Infrastructure;
using gpconnect_appointment_checker.Configuration.Infrastructure.Authentication;
using gpconnect_appointment_checker.Core.Configuration.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));

            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }        

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();

            var authenticationExtensions = new AuthenticationExtensions(_configuration);
            authenticationExtensions.ConfigureAuthenticationServices(services);

            services.ConfigureApplicationServices(_configuration, _webHostEnvironment);
            services.ConfigureLoggingServices(_configuration);
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