using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IDataService
    {
        List<T> ExecuteFunction<T>(string schemaName, string functionName) where T : class;
        List<T> ExecuteFunction<T>(string schemaName, string functionName, DynamicParameters parameters) where T : class;
        int ExecuteFunction(string schemaName, string functionName, DynamicParameters parameters);
        Task<List<T>> ExecuteFunctionAsync<T>(string schemaName, string functionName, DynamicParameters parameters, CancellationToken cancellationToken) where T : class;
        Task<int> ExecuteFunctionAsync(string schemaName, string functionName, DynamicParameters parameters, CancellationToken cancellationToken);
    }
}
