using gpconnect_appointment_checker.Configuration.Infrastructure;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;

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
            AddAuthenticationServices(services); 
            services.ConfigureApplicationServices(Configuration, WebHostEnvironment);
            services.ConfigureLoggingServices(Configuration);
            MappingExtensions.ConfigureMappingServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor contextAccessor, IAuditService auditService, IApplicationService applicationService)
        {
            _applicationService = applicationService;
            app.ConfigureApplicationBuilderServices(env);
        }

        public void AddAuthenticationServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(throwExceptionIfEmpty: true);
                options.DefaultSignOutScheme = Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(throwExceptionIfEmpty: true);
            }).AddCookie(s => {
                s.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                s.SlidingExpiration = true;
                })
            .AddOpenIdConnect(
                Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(throwExceptionIfEmpty: true),
                displayName: Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(throwExceptionIfEmpty: true),
                options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.ResponseMode = OpenIdConnectResponseMode.FormPost;
                    options.Authority = Configuration.GetSection("SingleSignOn:auth_endpoint").GetConfigurationString(throwExceptionIfEmpty: true);
                    options.MetadataAddress = Configuration.GetSection("SingleSignOn:metadata_endpoint").GetConfigurationString(throwExceptionIfEmpty: true);
                    options.ClientId = Configuration.GetSection("SingleSignOn:client_id").GetConfigurationString(throwExceptionIfEmpty: true);
                    options.ClientSecret = Configuration.GetSection("SingleSignOn:client_secret").GetConfigurationString(throwExceptionIfEmpty: true);
                    options.CallbackPath = Configuration.GetSection("SingleSignOn:callback_path").GetConfigurationString(throwExceptionIfEmpty: true);
                    //options.RemoteSignOutPath = "/Public/Index";
                    //options.SignedOutRedirectUri = "/Public/Index";
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.Scope.Add("email");
                    options.Scope.Add("profile");
                    options.Scope.Add("openid");
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var ldapTokenService = new LdapTokenService(services.BuildServiceProvider());
                            var tokenValidation = ldapTokenService.ExecutionTokenValidation(context);
                            return tokenValidation;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception == null)
                            {
                                context.Response.Redirect("/AccessDenied");
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                            throw context.Exception;
                        }
                    };
                });
        }

        
    }
}
