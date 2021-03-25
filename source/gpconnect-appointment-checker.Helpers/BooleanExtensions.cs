using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class BooleanExtensions
    {
        public static bool StringToBoolean(this string valueIn, bool defaultValue)
        {
            return bool.TryParse(valueIn, out _) ? Convert.ToBoolean(valueIn) : defaultValue;
        }

        public static string BooleanToYesNo(this bool input) =>
            input switch
            {
                _ => input ? "Yes" : "No"
            };
    }
}
