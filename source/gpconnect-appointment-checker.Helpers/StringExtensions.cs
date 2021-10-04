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

        public static string PractitionerBuilder(List<string> practitionerDetails, string familyName, string givenName, string prefix)
        {
            practitionerDetails ??= new List<string>();
            if (!string.IsNullOrEmpty(familyName)) { practitionerDetails.Add($"{familyName.ToUpper()},"); }
            if (!string.IsNullOrEmpty(givenName)) { practitionerDetails.Add($"{givenName}"); }
            if (!string.IsNullOrEmpty(prefix)) { practitionerDetails.Add($"({prefix})"); }
            return string.Join(" ", practitionerDetails);
        }

        public static string FlattenStrings(params string[] strings)
        {
            return string.Join(", ", strings.Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string Pluraliser(this string input, int countValue, string startTag = "", string endTag = "") =>
            startTag + input switch
            {
                null => string.Empty,
                "" => string.Empty,
                _ => countValue == 1 ? string.Format(input, countValue, string.Empty) : countValue == 0 ? string.Empty : string.Format(input, countValue, "s")
            } + endTag;

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
