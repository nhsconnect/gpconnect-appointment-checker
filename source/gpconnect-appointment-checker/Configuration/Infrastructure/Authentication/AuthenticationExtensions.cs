using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public static class AuthenticationExtensions
    {
        public static void ConfigureAuthenticationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = configuration.GetSection("SingleSignOn:challenge_scheme")
                        .GetConfigurationString(throwExceptionIfEmpty: true);
                    options.DefaultSignOutScheme = configuration.GetSection("SingleSignOn:challenge_scheme")
                        .GetConfigurationString(throwExceptionIfEmpty: true);
                }).AddCookie(s =>
                {
                    s.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    s.SlidingExpiration = true;
                    s.Cookie.SameSite = SameSiteMode.None;
                    s.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    s.Events.OnValidatePrincipal = PrincipalValidator.ValidateAsync;
                })
                .AddOpenIdConnect(
                    configuration.GetSection("SingleSignOn:challenge_scheme")
                        .GetConfigurationString(throwExceptionIfEmpty: true),
                    displayName: configuration.GetSection("SingleSignOn:challenge_scheme")
                        .GetConfigurationString(throwExceptionIfEmpty: true),
                    options =>
                    {
                        options.RequireHttpsMetadata = true;
                        options.ResponseMode = OpenIdConnectResponseMode.FormPost;
                        options.Authority = configuration.GetSection("SingleSignOn:auth_endpoint")
                            .GetConfigurationString(throwExceptionIfEmpty: true);
                        options.MetadataAddress = configuration.GetSection("SingleSignOn:metadata_endpoint")
                            .GetConfigurationString(throwExceptionIfEmpty: true);
                        options.ClientId = configuration.GetSection("SingleSignOn:client_id")
                            .GetConfigurationString(throwExceptionIfEmpty: true);
                        options.ClientSecret = configuration.GetSection("SingleSignOn:client_secret")
                            .GetConfigurationString(throwExceptionIfEmpty: true);
                        options.CallbackPath = configuration.GetSection("SingleSignOn:callback_path")
                            .GetConfigurationString(throwExceptionIfEmpty: true);
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
                                }

                                context.Response.Redirect("/Error");
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                        };
                    });
        }
    }
}
