using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthenticationExtensions
    {
        public SingleSignOnConfig _singleSignOnConfig { get; private set; }

        public AuthenticationExtensions(IConfiguration config)
        {
            config = config ?? throw new ArgumentNullException(nameof(config));
            _singleSignOnConfig = config.GetSection("SingleSignOnConfig").Get<SingleSignOnConfig>();
        }

        public void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = _singleSignOnConfig.AuthScheme;
                options.DefaultSignInScheme = _singleSignOnConfig.AuthScheme;
                options.DefaultChallengeScheme = _singleSignOnConfig.ChallengeScheme;
            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.LoginPath = "/Public/Index";
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = PrincipalValidator.ValidateAsync;
                options.Cookie.Name = ".GpConnectAppointmentChecker.AuthenticationCookie";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            })
            .AddOpenIdConnect(
                _singleSignOnConfig.ChallengeScheme,
                displayName: _singleSignOnConfig.ChallengeScheme,
                options =>
                {
                    options.SignInScheme = _singleSignOnConfig.AuthScheme;
                    options.Authority = _singleSignOnConfig.AuthEndpoint;
                    options.MetadataAddress = _singleSignOnConfig.MetadataEndpoint;
                    options.MaxAge = TimeSpan.FromMinutes(30);
                    options.SaveTokens = true;
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.ClientId = _singleSignOnConfig.ClientId;
                    options.ClientSecret = _singleSignOnConfig.ClientSecret;
                    options.CallbackPath = _singleSignOnConfig.CallbackPath;
                    options.SignedOutCallbackPath = _singleSignOnConfig.SignedOutCallbackPath;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationExtensions>>();
                            logger.LogInformation("Redirecting to OIDC provider with ClientId: {clientId}", _singleSignOnConfig.ClientId);
                            return Task.CompletedTask;
                        },
                        OnSignedOutCallbackRedirect = context =>
                        {
                            context.Response.Redirect(context.Options.SignedOutRedirectUri);
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                            return tokenService.HandleOnTokenValidatedAsync(context);
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception == null)
                            {
                                context.Response.Redirect("/AccessDenied");
                            }

                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthenticationExtensions>>();
                            logger.LogError(context.Exception?.StackTrace);

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
