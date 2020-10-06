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
using NLog;
using NLog.Targets;
using System.Data;

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
            services.AddControllersWithViews();
            AddLoggingAndConfigure(services);
            AddScopedServices(services);
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });
            AddAuthenticationServices(services);
            AddDapperMappings();
            AddGeneralConfiguration(services);
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
                Layout = "GpConnectAppointmentChecker",
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
                Layout = "${exception:format=stackTrace}",
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
                Layout =
                    "${date}|${level:uppercase=true}|${message}|${logger}|${callsite:filename=true}|${exception:format=stackTrace}|${var:userId}|${var:userSessionId}"
            };
            return consoleTarget;
        }

        private void AddDapperMappings()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new SsoMap());
                config.AddMap(new SdsQueryMap());
                config.AddMap(new GeneralMap());
                config.AddMap(new SpineMap());
                config.AddMap(new SpineMessageTypeMap());
            });
        }

        private async void AddGeneralConfiguration(IServiceCollection services)
        {
            var generalConfiguration = await ConfigurationService.GetGeneralConfiguration();
        }

        private async void AddAuthenticationServices(IServiceCollection services)
        {
            var ssoConfiguration = await ConfigurationService.GetSsoConfiguration();
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = ssoConfiguration.ChallengeScheme;
            //}).AddOAuth(ssoConfiguration.AuthScheme, options =>
            //{
            //    options.ClientId = ssoConfiguration.ClientId;
            //    options.ClientSecret = ssoConfiguration.ClientSecret;
            //    options.CallbackPath = new PathString(ssoConfiguration.CallbackPath);
            //    options.AuthorizationEndpoint = ssoConfiguration.AuthEndpoint;
            //    options.TokenEndpoint = ssoConfiguration.TokenEndpoint;
            //});
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
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
                endpoints.MapControllers();
            });
        }
    }
}
