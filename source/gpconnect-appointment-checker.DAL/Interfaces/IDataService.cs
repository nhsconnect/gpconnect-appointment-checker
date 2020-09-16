using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IDataService
    {
        Task<List<T>> ExecuteFunction<T>(string functionName) where T : class;
        Task<List<T>> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class;
        Task<int> ExecuteFunction(string functionName, DynamicParameters parameters);
    }
}
