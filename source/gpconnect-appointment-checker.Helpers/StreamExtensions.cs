using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers
{
    public static class StreamExtensions
    {
        public static T DeserializeJsonFromStream<T>(this Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default;

            using (var reader = new StreamReader(stream))
            {
                using (var jr = new JsonTextReader(reader))
                {
                    var d = new JsonSerializer().Deserialize<T>(jr);
                    return d;
                }
            }
        }

        public static string StreamToString(this Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = sr.ReadToEnd();

            return content;
        }

        public static string ConvertObjectToJsonData<T>(this T inputObject)
        {
            var json = JsonConvert.SerializeObject(inputObject);
            return json;
        }

        public static DataTable ConvertJsonDataToDataTable(this string inputObject)
        {
            var dataTable = (DataTable)JsonConvert.DeserializeObject(inputObject, typeof(DataTable));
            return dataTable;
        }
    }
}
