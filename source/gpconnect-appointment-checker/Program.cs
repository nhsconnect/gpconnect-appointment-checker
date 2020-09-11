using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    if (builderContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }
                }).ConfigureAppConfiguration(AddDbConfiguration);

        private static void AddDbConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
        {
            var configuration = builder.Build();

            builder.GetCustomConfiguration(options =>
            {
                options.Configuration = configuration.GetSection("connectionStrings:Configuration").Value;
                options.Query = configuration.GetSection("connectionStrings:Query").Value;
            });
        }
    }
}
