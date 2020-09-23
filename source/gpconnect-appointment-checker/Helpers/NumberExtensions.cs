using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class NumberExtensions
    {
        public static int StringToInteger(this string valueIn, int defaultValue)
        {
            return int.TryParse(valueIn, out _) ? Convert.ToInt32(valueIn) : defaultValue;
        }
    }

}
