using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthenticationExtensions
    {
        public Sso _ssoConfig { get; private set; }

        public AuthenticationExtensions(IConfiguration config)
        {
            _ssoConfig = config.GetSection("SingleSignOn").Get<Sso>();
        }

        public void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = _ssoConfig.AuthScheme;
                options.DefaultSignInScheme = _ssoConfig.AuthScheme;
                options.DefaultChallengeScheme = _ssoConfig.ChallengeScheme;
            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.LoginPath = "/Public/Index";
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = PrincipalValidator.ValidateAsync;
                options.Cookie.Name = ".GpConnectAppointmentChecker.AuthenticationCookie";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(
                _ssoConfig.ChallengeScheme,
                displayName: _ssoConfig.ChallengeScheme,
                options =>
                {
                    options.SignInScheme = _ssoConfig.AuthScheme;
                    options.Authority = _ssoConfig.AuthEndpoint;
                    options.MetadataAddress = _ssoConfig.MetadataEndpoint;
                    options.MaxAge = TimeSpan.FromMinutes(30);
                    options.SaveTokens = true;
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.ClientId = _ssoConfig.ClientId;
                    options.ClientSecret = _ssoConfig.ClientSecret;
                    options.CallbackPath = _ssoConfig.CallbackPath;
                    options.SignedOutCallbackPath = _ssoConfig.SignedOutCallbackPath;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnSignedOutCallbackRedirect = context =>
                        {
                            context.Response.Redirect(context.Options.SignedOutRedirectUri);
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var ldapTokenService = context.HttpContext.RequestServices.GetRequiredService<ILdapTokenService>();
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
                        },
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/Error");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
