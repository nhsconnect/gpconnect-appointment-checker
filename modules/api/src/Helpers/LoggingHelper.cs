using NLog;

namespace GpConnect.AppointmentChecker.Api.Helpers
{
    public static class LoggingHelper
    {
        public static int GetIntegerValue(string header)
        {
            if(LogManager.Configuration.Variables[header] != null)
            {
                return LogManager.Configuration.Variables[header].ToString().StringToInteger();
            }
            return 0;
        }

        public static string GetStringValue(string header)
        {
            if (LogManager.Configuration.Variables[header] != null)
            {
                return LogManager.Configuration.Variables[header].ToString();
            }
            return string.Empty;
        }
    }
}
