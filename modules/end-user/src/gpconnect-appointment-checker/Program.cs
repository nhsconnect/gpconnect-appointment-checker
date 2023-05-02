using Autofac.Extensions.DependencyInjection;
using GpConnect.AppointmentChecker.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                .ConfigureWebHost(o =>
                {
                    o.CaptureStartupErrors(true).UseSetting("detailedErrors", "");
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    });
                }).ConfigureAppConfiguration(CustomConfigurationBuilder.AddCustomConfiguration)
                .ConfigureLogging((builderContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                }).UseNLog();

        //private static void AddCustomConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        //{
        //    builder.AddEnvironmentVariables("GPCONNECTAPPOINTMENTCHECKER_");
        //    builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //    var configuration = builder.Build();
        //    //builder.AddConfiguration(options =>
        //    //{
        //    //    options.ConnectionString = configuration.GetConnectionString(ConnectionStrings.DefaultConnection);
        //    //});
        //}
    }
}
