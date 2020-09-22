using Dapper.FluentMap;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Audit;
using gpconnect_appointment_checker.DAL.Configuration;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Logging;
using gpconnect_appointment_checker.DAL.Mapping;
using gpconnect_appointment_checker.GPConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Logging.GlobalExceptionHandler;
using gpconnect_appointment_checker.SDS;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddScopedServices(services);
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = true;
            });
            AddAuthenticationServices(services);
            AddDapperMappings();
        }

        private void AddDapperMappings()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new SsoMap());
                config.AddMap(new SdsQueryMap());
                config.AddMap(new GeneralMap());
                config.AddMap(new SpineMap());
            });
        }

        private async void AddAuthenticationServices(IServiceCollection services)
        {
            var configurationService = new ConfigurationService(Configuration);
            var ssoConfiguration = await configurationService.GetSsoConfiguration();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = ssoConfiguration.ChallengeScheme;
            }).AddCookie()
            .AddOAuth(ssoConfiguration.AuthScheme, options =>
            {
                options.ClientId = ssoConfiguration.ClientId;
                options.ClientSecret = ssoConfiguration.ClientSecret;
                options.CallbackPath = new PathString(ssoConfiguration.CallbackPath);
                options.AuthorizationEndpoint = ssoConfiguration.AuthEndpoint;
                options.TokenEndpoint = ssoConfiguration.TokenEndpoint;
            });
        }

        private void AddScopedServices(IServiceCollection services)
        {
            services.AddScoped<ILdapService, LdapService>();
            services.AddScoped<ISDSQueryExecutionService, SDSQueryExecutionService>();
            services.AddScoped<IGPConnectQueryExecutionService, GPConnectQueryExecutionService>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ILogService, LogService>();
            services.AddSingleton<ILog, LogNLog>();
        }

        public void Configure(IApplicationBuilder app, ILog logger, IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            app.ConfigureExceptionHandler(logger);
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
