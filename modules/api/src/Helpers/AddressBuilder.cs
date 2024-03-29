﻿namespace GpConnect.AppointmentChecker.Api.Helpers;
public static class AddressBuilder
{
    public static string GetAddress(List<string> addressLines, string postalCode = "")
    {
        if (addressLines != null)
        {
            addressLines ??= new List<string>();
            if (!string.IsNullOrEmpty(postalCode))
            {
                addressLines.Add(postalCode);
            }
            return string.Join(", ", addressLines.Where(s => !string.IsNullOrEmpty(s?.Trim())).Select(x => x?.Trim()));
        }
        return string.Empty;
    }

    public static string GetFullAddress(List<string> addressLines, string district, string city, string postalCode, string country)
    {
        addressLines ??= new List<string>();
        addressLines.Add(district);
        addressLines.Add(city);
        addressLines.Add(postalCode);
        addressLines.Add(country);
        return string.Join(", ", addressLines.Where(s => !string.IsNullOrEmpty(s?.Trim())).Select(x => x?.Trim()));
    }
}
