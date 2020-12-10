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
    }
}
