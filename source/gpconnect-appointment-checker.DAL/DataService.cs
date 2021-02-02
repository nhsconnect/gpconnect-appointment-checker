using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;

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

        public List<T> ExecuteFunction<T>(string functionName) where T : class
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var results = (connection.Query<T>(functionName, null, commandType: System.Data.CommandType.StoredProcedure)).AsList();
                return results;
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, $"An error has occurred while attempting to execute the function {functionName}");
                throw;
            }
        }

        public List<T> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var results = (connection.Query<T>(functionName, parameters, commandType: System.Data.CommandType.StoredProcedure)).AsList();
                return results;
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, $"An error has occurred while attempting to execute the function {functionName}");
                throw;
            }
        }

        public int ExecuteFunction(string functionName, DynamicParameters parameters)
        {
            try
            {
                using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
                var rowsInserted = connection.Execute(functionName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                return rowsInserted;
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, $"An error has occurred while attempting to execute the function {functionName}");
                throw;
            }
        }
    }
}
