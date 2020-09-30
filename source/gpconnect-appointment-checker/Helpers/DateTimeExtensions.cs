using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime valueIn, DayOfWeek startOfWeek) =>
            valueIn switch
            {
                _ => valueIn.AddDays(-1 * (7 + (valueIn.DayOfWeek - startOfWeek)) % 7).Date
            };
        
        public static string DateFormatter(this DateTime? valueIn, string dateFormat)
        {
            return valueIn.HasValue ? valueIn.Value.ToString(dateFormat) : string.Empty;
        }
    }
}
