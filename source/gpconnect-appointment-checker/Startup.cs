using Dapper.FluentMap;
using gpconnect_appointment_checker.Configuration;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Mapping;
using gpconnect_appointment_checker.DTO.Request.Application;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Targets;
using System;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString(ConnectionStrings.DefaultConnection)))
                throw new ArgumentException($"Environment variable ConnectionStrings:{ConnectionStrings.DefaultConnection} is not present");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ILdapService _ldapService { get; set; }
        public IApplicationService _applicationService { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            AddLoggingAndConfigure(services);
            services.AddHttpContextAccessor();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Private");
                options.Conventions.AllowAnonymousToFolder("/Public");
                options.Conventions.AddPageRoute("/Private/Search", "/Search");
                options.Conventions.AddPageRoute("/Public/AccessDenied", "/AccessDenied");
                options.Conventions.AddPageRoute("/Public/Index", "");
            });
            services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });
            AddAuthenticationServices(services);
            services.AddAuthorization();
            AddDapperMappings();
        }

        private void AddLoggingAndConfigure(IServiceCollection services)
        {
            var nLogConfiguration = new NLog.Config.LoggingConfiguration();

            var consoleTarget = AddConsoleTarget();
            var databaseTarget = AddDatabaseTarget();

            nLogConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
            nLogConfiguration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, databaseTarget);

            nLogConfiguration.AddTarget(consoleTarget);
            nLogConfiguration.AddTarget(databaseTarget);

            nLogConfiguration.Variables.Add("applicationVersion", ApplicationVersion.GetAssemblyVersion);

            LogManager.Configuration = nLogConfiguration;

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(Configuration.GetSection("Logging"));
            });
        }

        private DatabaseTarget AddDatabaseTarget()
        {
            var databaseTarget = new DatabaseTarget
            {
                Name = "Database",
                ConnectionString = Configuration.GetConnectionString("DefaultConnection"),
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO logging.error_log (Application, Logged, Level, Message, Logger, CallSite, Exception, User_Id, User_Session_Id) VALUES (@Application, @Logged, @Level, @Message, @Logger, @Callsite, @Exception, @UserId, @UserSessionId)",
                DBProvider = "Npgsql.NpgsqlConnection, Npgsql"
            };

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Application",
                Layout = "${var:applicationVersion}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Logged",
                Layout = "${date}",
                DbType = DbType.DateTime.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Level",
                Layout = "${level:uppercase=true}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Level",
                Layout = "${level:uppercase=true}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Message",
                Layout = "${message}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Logger",
                Layout = "${logger}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Callsite",
                Layout = "${callsite:filename=true}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@Exception",
                Layout = "${exception:format=type,message,method,stacktrace:maxInnerExceptionLevel=10:innerFormat=shortType,message,method,stacktrace}",
                DbType = DbType.String.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@UserId",
                Layout = "${var:userId}",
                DbType = DbType.Int32.ToString()
            });

            databaseTarget.Parameters.Add(new DatabaseParameterInfo
            {
                Name = "@UserSessionId",
                Layout = "${var:userSessionId}",
                DbType = DbType.Int32.ToString()
            });
            return databaseTarget;
        }

        private static ConsoleTarget AddConsoleTarget()
        {

            var consoleTarget = new ConsoleTarget
            {
                Name = "Console",
                Layout = "${var:applicationVersion}|${date}|${level:uppercase=true}|${message}|${logger}|${callsite:filename=true}|${exception:format=stackTrace}|${var:userId}|${var:userSessionId}"
            };
            return consoleTarget;
        }

        private void AddDapperMappings()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new SdsQueryMap());
                config.AddMap(new SpineMessageTypeMap());
                config.AddMap(new UserMap());
                config.AddMap(new OrganisationMap());
            });
        }

        private void AddAuthenticationServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString();
                options.DefaultSignOutScheme = Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString();
            }).AddCookie().AddOpenIdConnect(Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(), displayName: Configuration.GetSection("SingleSignOn:challenge_scheme").GetConfigurationString(), options =>
            {
                options.Authority = Configuration.GetSection("SingleSignOn:auth_endpoint").GetConfigurationString();
                options.MetadataAddress = Configuration.GetSection("SingleSignOn:metadata_endpoint").GetConfigurationString();
                options.ClientId = Configuration.GetSection("SingleSignOn:client_id").GetConfigurationString();
                options.ClientSecret = Configuration.GetSection("SingleSignOn:client_secret").GetConfigurationString();
                options.CallbackPath = Configuration.GetSection("SingleSignOn:callback_path").GetConfigurationString();
                options.Scope.Add("email");
                options.Scope.Add("profile");
                options.Scope.Add("openid");
                options.SignedOutCallbackPath = "/Test";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var organisationDetails = await _ldapService.GetOrganisationDetailsByOdsCode(context.Principal.GetClaimValue("ODS"));
                        var organisation = await _applicationService.GetOrganisation(organisationDetails.ODSCode);
                        var loggedOnUser = await _applicationService.LogonUser(new User
                        {
                            EmailAddress = context.Principal.GetClaimValue("Email"),
                            DisplayName = context.Principal.GetClaimValue("DisplayName"),
                            OrganisationId = organisation.OrganisationId
                        });

                        if (!loggedOnUser.IsAuthorised)
                        {
                            context.Response.Redirect("/AccessDenied");
                            context.HandleResponse();
                        }
                        else
                        {
                            if (context.Principal.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("OrganisationName", organisationDetails.OrganisationName));
                                identity.AddClaim(new Claim("UserSessionId", loggedOnUser.UserSessionId.ToString()));
                            }
                        }
                    },
                    OnRedirectToIdentityProviderForSignOut = async context =>
                    {
                        await _applicationService.LogoffUser(new User
                        {
                            EmailAddress = context.HttpContext.User.GetClaimValue("Email"),
                            UserSessionId = context.HttpContext.User.GetClaimValue("UserSessionId").StringToInteger(0)
                        });
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/AccessDenied");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor contextAccessor, ILdapService ldapService, IAuditService auditService, IApplicationService applicationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            _ldapService = ldapService;
            _applicationService = applicationService;

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
