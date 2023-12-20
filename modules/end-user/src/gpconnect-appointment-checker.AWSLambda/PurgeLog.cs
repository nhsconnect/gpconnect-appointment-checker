using Amazon.Lambda.Core;
using Dapper;
using gpconnect_appointment_checker.DTO.Response.Logging;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Linq;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace gpconnect_appointment_checker.AWSLambda
{
    public class Function
    {
        public Function()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.BuildServiceProvider();
        }

        public void PurgeLogFunctionHandler(ILambdaContext context)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            var functionName = "logging.purge_logs";
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                var results = connection.Query<PurgeErrorLog>(functionName, null, commandType: System.Data.CommandType.StoredProcedure).AsList();
                LambdaLogger.Log($"Error Log Deleted Count - {results.FirstOrDefault()?.error_log_deleted_count}");
                LambdaLogger.Log($"Spine Message Deleted Count - {results.FirstOrDefault()?.spine_message_deleted_count}");
                LambdaLogger.Log($"Web Request Deleted Count - {results.FirstOrDefault()?.web_request_deleted_count}");
            }
            catch (Exception exc)
            {
                context.Logger.LogLine($"An error has occurred while attempting to execute the function {functionName} - {exc}");
                throw;
            }
        }
    }
}
