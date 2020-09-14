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
                })
                .ConfigureLogging((builderContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.AddConfiguration(builderContext.Configuration.GetSection("Logging"));
                    }
                ).UseNLog()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddEnvironmentVariables();
                }).ConfigureAppConfiguration(AddDbConfiguration);

        private static void AddDbConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();
            //builder.GetCustomConfiguration(options =>
            //{
            //    options.Configuration = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            //});
        }
    }
}
