using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class DateTimeExtensions
    {
        public static double DurationBetweenTwoDates(this DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            if (startDate == null || endDate == null) return 0;
            var durationTimeSpan = endDate - startDate;
            return durationTimeSpan.Value.TotalMinutes;
        }
    }
}
