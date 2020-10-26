using gpconnect_appointment_checker.Configuration;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Application;
using gpconnect_appointment_checker.DAL.Audit;
using gpconnect_appointment_checker.DAL.Configuration;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Logging;
using gpconnect_appointment_checker.GPConnect;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.SDS;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace gpconnect_appointment_checker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(o => o.CaptureStartupErrors(true).UseSetting("detailedErrors", ""))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddScoped<ISDSQueryExecutionService, SDSQueryExecutionService>();
                        services.AddScoped<IGPConnectQueryExecutionService, GPConnectQueryExecutionService>();
                        services.AddScoped<IDataService, DataService>();
                        services.AddScoped<IConfigurationService, ConfigurationService>();
                        services.AddScoped<IAuditService, AuditService>();
                        services.AddScoped<ITokenService, TokenService>();
                        services.AddScoped<IApplicationService, ApplicationService>();
                        services.AddScoped<ILdapService, LdapService>();
                        services.AddScoped<ILogService, LogService>();
                        services.AddHttpClient();
                    });
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "GPCONNECTAPPOINTMENTCHECKER_");
                }).ConfigureAppConfiguration(AddCustomConfiguration)
                .ConfigureLogging((builderContext, logging) => {
                    logging.ClearProviders();
                    logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                }).UseNLog();

        private static void AddCustomConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            builder.AddConfiguration(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
            });
        }
    }
}
