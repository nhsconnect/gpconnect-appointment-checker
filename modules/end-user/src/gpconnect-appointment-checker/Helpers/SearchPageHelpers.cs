using System;
using DocumentFormat.OpenXml.EMMA;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Helpers;

public static class SearchPageHelpers
{
    public static string BuildSearchTimeCountString(int count, TimeProvider timeProvider,
        ITimeZoneProvider timeZoneProvider)
    {
        var utcNow = timeProvider.GetUtcNow();
        var ukTimeZone = timeZoneProvider.FindSystemTimeZoneById("Europe/London");
        var currentUkTimeOffset = timeZoneProvider.ConvertTime(utcNow, ukTimeZone);

        var formattedUkTime = currentUkTimeOffset.DateTime.ToString("dd MMM HH:mm");

        return
            $"{string.Format(SearchConstants.SearchAtDate, formattedUkTime)} - {SearchConstants.SearchStatsCountText.Pluraliser(count)}";
    }
}