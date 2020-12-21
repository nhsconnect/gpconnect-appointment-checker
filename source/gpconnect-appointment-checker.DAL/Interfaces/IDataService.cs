using Dapper;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IDataService
    {
        List<T> ExecuteFunction<T>(string functionName) where T : class;
        List<T> ExecuteFunction<T>(string functionName, DynamicParameters parameters) where T : class;
        int ExecuteFunction(string functionName, DynamicParameters parameters);
    }
}
