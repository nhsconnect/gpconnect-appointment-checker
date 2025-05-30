using System;

namespace gpconnect_appointment_checker.Helpers;

public interface ITimeZoneProvider
{
    TimeZoneInfo FindSystemTimeZoneById(string id);
    DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone);
}