using Amazon.Lambda.Core;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace gpconnect_appointment_checker.AWSLambda
{
    public class Function
    {
        private ILogService LogService { get; }
        private ILambdaConfiguration Configuration { get; }

        public Function()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Configuration = serviceProvider.GetService<ILambdaConfiguration>();
            LogService = serviceProvider.GetService<ILogService>();
        }

        public async Task<DTO.Response.Logging.PurgeErrorLog> PurgeLogFunctionHandler(ILambdaContext context)
        {
            context.Logger.LogLine($"Started execution for function - {context.FunctionName} at {DateTime.Now}");
            var response = await LogService.PurgeLogEntries();
            var result = response.FirstOrDefault();
            context.Logger.LogLine($"Finished execution for function - {context.FunctionName} at {DateTime.Now}");
            LambdaLogger.Log($"Error log entries removed - {result?.ErrorLogDeletedCount}");
            LambdaLogger.Log($"Spine message log entries removed - {result?.SpineMessageDeletedCount}");
            LambdaLogger.Log($"Web request log entries removed - {result?.WebRequestDeletedCount}");
            return result;
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ILogService, LogService>();
            serviceCollection.AddTransient<IDataService, DataService>();
            serviceCollection.AddTransient<ILambdaConfiguration, LambdaConfiguration>();
        }
    }
}
