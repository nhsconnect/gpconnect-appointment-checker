using Newtonsoft.Json;
using System.IO;

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

        //public static T DeserializeJsonFromStream<T>(this Stream stream)
        //{
        //    if (stream == null || stream.CanRead == false)
        //        return default;

        //    using var sr = new StreamReader(stream);
        //    using var jtr = new JsonTextReader(sr);
        //    var js = new JsonSerializer();
        //    var searchResult = js.Deserialize<T>(jtr);
        //    return searchResult;
        //}

        public static string StreamToString(this Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = sr.ReadToEnd();

            return content;
        }
    }
}
