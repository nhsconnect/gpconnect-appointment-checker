using Dapper;
using System.Collections.Generic;
using System.Data;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IDataService
    {
        List<T> ExecuteFunction<T>(string functionName) where T : class;
        DataTable ExecuteFunctionAndGetDataTable(string functionName);
        List<T> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class;
        int ExecuteFunction(string functionName, DynamicParameters parameters);
    }
}
