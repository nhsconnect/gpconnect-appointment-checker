using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static string DurationFormatter(this int duration, string durationUnits)
        {
            return $"{duration} {durationUnits}";
        }
    }
}
