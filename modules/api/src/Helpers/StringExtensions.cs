﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Api.Helpers;

public static class StringExtensions
{
    public static string ReplaceNonAlphanumeric(this string input, string replacementCharacter = "_") =>
        input switch
        {
            null or "" => string.Empty,
            _ => Regex.Replace(input, "[^a-zA-Z0-9]", replacementCharacter)
        };

    public static string FirstCharToUpper(this string input, bool restToLower = false) =>
        input switch
        {
            null => string.Empty,
            "" => string.Empty,
            _ => input.First().ToString().ToUpper() + (restToLower ? input.Substring(1).ToLower() : input.Substring(1))
        };

    // TODO: This is not coalescing - needs removed or renamed
    public static string Coalesce(params string[] strings)
    {
        return strings.FirstOrDefault(s => !string.IsNullOrEmpty(s));
    }

    public static string SearchAndReplace(this string input, Dictionary<string, string> replacementValues) =>
        input switch
        {
            null or "" => string.Empty,
            _ => replacementValues.Aggregate(input, (current, value) => current.Replace(value.Key, value.Value))
        };

    public static string FlattenStrings(params string[] strings)
    {
        return string.Join(", ", strings.Where(s => !string.IsNullOrEmpty(s)));
    }

    public static string ToTitleCase(this string input) => input?.Trim() switch
    {
        null or "" => string.Empty,
        _ => new CultureInfo("en-GB", false).TextInfo.ToTitleCase(input.ToLower())
    };

    public static string Pluraliser(this string input, int countValue, string startTag = "", string endTag = "") =>
        startTag + input switch
        {
            null => string.Empty,
            "" => string.Empty,
            _ => countValue == 1 ? string.Format(input, countValue, string.Empty) :
                countValue == 0 ? string.Empty : string.Format(input, countValue, "s")
        } + endTag;

    public static bool IsJson(this string input)
    {
        try
        {
            JObject.Parse(input);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
}