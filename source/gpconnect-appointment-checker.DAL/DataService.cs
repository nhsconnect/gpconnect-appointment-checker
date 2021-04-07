using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NpgsqlTypes;

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

        public DataTable ExecuteFunctionAndGetDataTable(string functionName)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
            connection.Open();
            using (var cmd = new NpgsqlCommand(functionName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Prepare();

                var da = new NpgsqlDataAdapter(cmd);
                var _ds = new DataSet();
                var _dt = new DataTable();

                da.Fill(_ds);

                try
                {
                    _dt = _ds.Tables[0];
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"An error has occurred while attempting to execute the function {functionName}");
                    throw;
                }

                connection.Close();
                return _dt;
            }
        }

        public DataTable ExecuteFunctionAndGetDataTable(string functionName, Dictionary<string, int> parameters)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(_configuration.GetConnectionString(ConnectionStrings.DefaultConnection));
            connection.Open();
            using (var cmd = new NpgsqlCommand(functionName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.AddWithValue(parameter.Key, NpgsqlDbType.Integer, parameter.Value);
                }

                cmd.Prepare();

                var da = new NpgsqlDataAdapter(cmd);
                var _ds = new DataSet();
                var _dt = new DataTable();

                da.Fill(_ds);

                try
                {
                    _dt = _ds.Tables[0];
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"An error has occurred while attempting to execute the function {functionName}");
                    throw;
                }

                connection.Close();
                return _dt;
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
