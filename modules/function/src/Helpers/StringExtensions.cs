using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Function.Helpers;

public static class StringExtensions
{
    public static string ReplaceNonAlphanumeric(this string input, string replacementCharacter = "_") =>
        input switch
        {
            null or "" => string.Empty,
            _ => Regex.Replace(input, "[^a-zA-Z0-9]", replacementCharacter)
        };

    public static string SearchAndReplace(this string input, Dictionary<string, string> replacementValues) =>
       input switch
       {
           null or "" => string.Empty,
           _ => replacementValues.Aggregate(input, (current, value) => current.Replace(value.Key, value.Value))
       };

}
