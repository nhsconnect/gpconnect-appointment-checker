namespace GpConnect.AppointmentChecker.Api.Helpers;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime valueIn, DayOfWeek startOfWeek) =>
        valueIn switch
        {
            _ => valueIn.AddDays(-1 * (7 + (valueIn.DayOfWeek - startOfWeek)) % 7).Date
        };

    public static string DateFormatter(this DateTimeOffset? valueIn, string dateFormat)
    {
        return valueIn.HasValue ? valueIn.Value.ToString(dateFormat) : string.Empty;
    }

    public static string TimeZoneConverter(this DateTime valueIn, string timeZoneId, string dateTimeFormat = "d MMM yyyy HH:mm:ss")
    {
        DateTime currentTimeZoneInfoLocal = valueIn;
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
        try
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);                
        }
        catch (TimeZoneNotFoundException)
        {
        }
        finally
        {
            currentTimeZoneInfoLocal = TimeZoneInfo.ConvertTime(valueIn, TimeZoneInfo.Local, timeZoneInfo);
        }
        return currentTimeZoneInfoLocal.ToString(dateTimeFormat);
    }

    public static DateTime TimeZoneConverter(this DateTime valueIn, string timeZoneId = "Europe/London")
    {
        DateTime currentTimeZoneInfoLocal = valueIn;
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
        try
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
        }
        finally
        {
            currentTimeZoneInfoLocal = TimeZoneInfo.ConvertTime(valueIn, TimeZoneInfo.Local, timeZoneInfo);
        }
        return currentTimeZoneInfoLocal;
    }

    public static double DurationBetweenTwoDates(this DateTimeOffset? startDate, DateTimeOffset? endDate)
    {
        if (startDate == null || endDate == null) return 0;
        var durationTimeSpan = endDate - startDate;
        return durationTimeSpan.Value.TotalMinutes;
    }
}
