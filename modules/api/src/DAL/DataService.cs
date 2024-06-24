using Dapper;
using GpConnect.AppointmentChecker.Api.Dal.Configuration;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.DAL;

public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;
    private readonly IOptions<ConnectionStrings> _optionsAccessor;

    public DataService(IOptions<ConnectionStrings> optionsAccessor, ILogger<DataService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _optionsAccessor = optionsAccessor ?? throw new ArgumentNullException(nameof(optionsAccessor));
    }

    public async Task<List<T>> ExecuteQuery<T>(string query, DynamicParameters? parameters = null) where T : class
    {
        try
        {
            CheckQuery(query);
            await using var connection = GetConnection();            
            var results = (await connection.QueryAsync<T>(query, parameters, commandType: CommandType.StoredProcedure)).AsList();
            return results;
        }
        catch (PostgresException exc)
        {
            _logger?.LogError(exc, "A database backend error has occurred while attempting to execute the query");
            throw;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, $"An error has occurred while attempting to execute the query {query}");
            throw;
        }
    }

    public async Task<T> ExecuteQueryFirstOrDefault<T>(string query, DynamicParameters? parameters = null) where T : class
    {
        try
        {
            CheckQuery(query);
            await using var connection = GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (PostgresException exc)
        {
            _logger?.LogError(exc, "A database backend error has occurred while attempting to execute the query");
            throw;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, $"An error has occurred while attempting to execute the query {query}");
            throw;
        }
    }

    public async Task<DataTable> ExecuteFunctionAndGetDataTable(string query, Dictionary<string, int>? parameters = null)
    {
        CheckQuery(query);
        await using var connection = GetConnection();
        connection.Open();
        using (var cmd = new NpgsqlCommand(query, connection))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.AddWithValue(parameter.Key, NpgsqlDbType.Integer, parameter.Value);
                }
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
            catch (PostgresException exc)
            {
                _logger?.LogError(exc, "A database backend error has occurred while attempting to execute the query");
                throw;
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, $"An error has occurred while attempting to execute the query {query}");
                throw;
            }

            connection.Close();
            return _dt;
        }
    }    

    public async Task<int> ExecuteQuery(string query, DynamicParameters? parameters = null)
    {
        try
        {
            CheckQuery(query);
            await using var connection = GetConnection();
            var rowsProcessed = await connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
            return rowsProcessed;
        }
        catch (PostgresException exc)
        {
            _logger?.LogError(exc, "A database backend error has occurred while attempting to execute the query");
            throw;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, $"An error has occurred while attempting to execute the query {query}");
            throw;
        }
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_optionsAccessor.Value.DefaultConnection);
    }

    private string CheckQuery(string query)
    {
        if (string.IsNullOrEmpty(query?.Trim()))
            throw new ArgumentNullException(nameof(query));
        return query;
    }
}
