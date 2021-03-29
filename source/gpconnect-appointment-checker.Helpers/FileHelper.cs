using System;
using System.IO;

namespace gpconnect_appointment_checker.Helpers
{
    public static class FileHelper
    {
        public static string ReadFileContents(string filename)
        {
            var path = $@"{AppContext.BaseDirectory}{filename}";

            if (File.Exists(path))
            {
                var readText = File.ReadAllText(path);
                return readText;
            }
            return null;
        }
    }
}
