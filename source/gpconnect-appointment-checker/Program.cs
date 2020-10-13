using gpconnect_appointment_checker.Configuration;
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "GPCONNECTAPPOINTMENTCHECKER_");
                    //config.AddMyConfiguration(options =>
                    //{
                    //    options.ConnectionString = "Server=localhost;Port=5432;Database=GpConnectAppointmentChecker;User Id=postgres;Password=hYrfbq74%Na$xFIe!QRA;";
                    //    options.Query = "SELECT * FROM configuration.spine";
                    //});
                }).ConfigureLogging((builderContext, logging) => {
                    logging.ClearProviders();
                    logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                }).UseNLog();
    }
}
