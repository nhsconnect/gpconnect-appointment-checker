using Dapper;
using Npgsql;
using System.Data;

namespace GpConnect.AppointmentChecker.Api.DAL.Interfaces;

public interface IDataService
{
    Task<List<T>> ExecuteQuery<T>(string query, DynamicParameters? parameters = null) where T : class;
    Task<T> ExecuteQueryFirstOrDefault<T>(string query, DynamicParameters? parameters = null) where T : class;
    Task<int> ExecuteQuery(string query, DynamicParameters parameters);
    Task<DataTable> ExecuteFunctionAndGetDataTable(string query, Dictionary<string, int>? parameters = null);
    NpgsqlConnection GetConnection();
}
