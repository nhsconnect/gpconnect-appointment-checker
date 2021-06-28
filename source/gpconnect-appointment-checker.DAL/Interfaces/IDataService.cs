using Dapper;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IDataService
    {
        List<T> ExecuteFunction<T>(string functionName) where T : class;
        DataTable ExecuteFunctionAndGetDataTable(string functionName);
        DataTable ExecuteFunctionAndGetDataTable(string functionName, Dictionary<string, int> parameters);
        List<T> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class;
        int ExecuteFunction(string functionName, DynamicParameters parameters);
        int ExecuteQuery(string query);
    }
}
