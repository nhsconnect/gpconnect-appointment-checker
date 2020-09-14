using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL
{
    public class DataInterface : DataSimple
    {
        private readonly ILogger<DataInterface> _logger;

        public DataInterface(string _connectionString) : base(_connectionString) { }

        public DataInterface(IConfiguration configuration, ILogger<DataInterface> logger) : base(configuration)
        {
            _logger = logger;
        }

        protected async Task<T> ExecuteFunction<T>(string functionName) where T : class
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                var results = await connection.QueryAsync<T>(functionName);
                return (T)results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
            }
            return null;
        }

        protected async Task<T> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                var results = await connection.QueryAsync<T>(functionName, parameters);
                return (T)results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
            }
            return null;
        }

        protected async Task<int> ExecuteFunction(string functionName, DynamicParameters parameters)
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                var rowsInserted = await connection.ExecuteAsync(functionName, parameters);
                return rowsInserted;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
            }
            return 0;
        }
    }
}
