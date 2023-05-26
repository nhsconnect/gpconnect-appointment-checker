using Newtonsoft.Json;
using System.Data;

namespace gpconnect_appointment_checker.Helpers
{
    public static class StreamExtensions
    {       
        public static DataTable ConvertJsonDataToDataTable(this string inputObject)
        {
            var dataTable = (DataTable)JsonConvert.DeserializeObject(inputObject, typeof(DataTable));
            return dataTable;
        }
    }
}
