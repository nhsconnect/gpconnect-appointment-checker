using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL
{
    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly IConfiguration _configuration;

        public DataService(IConfiguration configuration, ILogger<DataService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<T>> ExecuteFunction<T>(string functionName) where T : class
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var results = (await connection.QueryAsync<T>(functionName, null, commandType: System.Data.CommandType.StoredProcedure)).AsList();
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
                throw;
            }
        }

        public async Task<List<T>> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var results = (await connection.QueryAsync<T>(functionName, parameters, commandType: System.Data.CommandType.StoredProcedure)).AsList();
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
                throw;
            }
        }

        public async Task<int> ExecuteFunction(string functionName, DynamicParameters parameters)
        {
            try
            {
                await using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var rowsInserted = connection.Execute(functionName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                return rowsInserted;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred", exc);
                throw;
            }
        }
    }
}
