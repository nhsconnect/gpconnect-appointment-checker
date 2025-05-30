using System;

namespace gpconnect_appointment_checker.Helpers;

public class DefaultTimeZoneProvider : ITimeZoneProvider
{
    public TimeZoneInfo FindSystemTimeZoneById(string id)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    public DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
    {
        return TimeZoneInfo.ConvertTime(dateTimeOffset, destinationTimeZone);
    }
}