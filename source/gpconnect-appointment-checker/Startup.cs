using System.Net.Http.Headers;
using Dapper.FluentMap;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Audit;
using gpconnect_appointment_checker.DAL.Configuration;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Logging;
using gpconnect_appointment_checker.DAL.Mapping;
using gpconnect_appointment_checker.GPConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.SDS;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IConfigurationService ConfigurationService => new ConfigurationService(Configuration);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
                builder.AddDebug()
                    .AddConsole().AddConfiguration(Configuration.GetSection("Logging"))
                    .SetMinimumLevel(LogLevel.Trace));

            AddScopedServices(services);
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = true;
            });
            AddAuthenticationServices(services);
            AddDapperMappings();
            AddGeneralConfiguration(services);
            
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

        private async void AddGeneralConfiguration(IServiceCollection services)
        {
            var generalConfiguration = await ConfigurationService.GetGeneralConfiguration();
        }

        private async void AddAuthenticationServices(IServiceCollection services)
        {
            var ssoConfiguration = await ConfigurationService.GetSsoConfiguration();

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
            services.AddScoped<ITokenService, TokenService>();
            services.AddHttpClient();
            //services.AddHttpClient<GPConnectQueryExecutionService>(c =>
            //{
            //    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

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
