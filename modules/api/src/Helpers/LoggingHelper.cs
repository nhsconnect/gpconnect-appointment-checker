using NLog;

namespace GpConnect.AppointmentChecker.Api.Helpers
{
    public static class LoggingHelper
    {
        public static int GetIntegerValue(string header)
        {
            if (LogManager.Configuration.Variables.Count > 0)
            {
                return LogManager.Configuration.Variables?[header] != null
                    ? LogManager.Configuration.Variables[header].ToString().StringToInteger(0)
                    : 0;
            }

            return 0;
        }

        public static string GetStringValue(string header)
        {
            return LogManager.Configuration.Variables[header] != null
                ? LogManager.Configuration.Variables[header].ToString()
                : string.Empty;
        }
    }
}