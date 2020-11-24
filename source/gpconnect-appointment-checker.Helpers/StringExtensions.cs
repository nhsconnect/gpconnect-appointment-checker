using System.Linq;

namespace gpconnect_appointment_checker.Helpers
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => string.Empty,
                "" => string.Empty,
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
    }
}
