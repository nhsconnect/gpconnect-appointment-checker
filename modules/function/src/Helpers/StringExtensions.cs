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
}
