using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input, bool restToLower = false) =>
            input switch
            {
                null => string.Empty,
                "" => string.Empty,
                _ => input.First().ToString().ToUpper() + (restToLower ? input.Substring(1).ToLower() : input.Substring(1))
            };

        public static string Coalesce(params string[] strings)
        {
            return strings.FirstOrDefault(s => !string.IsNullOrEmpty(s));
        }

        public static string AddressBuilder(List<string> addressLines, string district, string city, string postalCode, string country)
        {
            addressLines ??= new List<string>();
            addressLines.Add(district);
            addressLines.Add(city);
            addressLines.Add(postalCode);
            addressLines.Add(country);
            return string.Join(", ", addressLines.Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string FlattenStrings(params string[] strings)
        {
            return string.Join(", ", strings.Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string Pluraliser(this string input, int countValue) =>
            input switch
            {
                null => string.Empty,
                "" => string.Empty,
                _ => countValue + countValue == 1 ? input : input + "s"
            };

        public static string AddressBuilder(List<string> addressLines, string postalCode)
        {
            if (addressLines != null && !string.IsNullOrEmpty(postalCode))
            {
                addressLines ??= new List<string>();
                addressLines.Add(postalCode);
                return string.Join(", ", addressLines.Where(s => !string.IsNullOrEmpty(s.Trim())));
            }
            return string.Empty;
        }
    }
}
