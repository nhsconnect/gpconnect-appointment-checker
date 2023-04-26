using Microsoft.Extensions.Configuration;
using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetConfigurationString(this IConfigurationSection configurationSetting, string defaultValue = "", bool throwExceptionIfEmpty = false)
        {
            var keyValueExists = configurationSetting.Exists() && !string.IsNullOrEmpty(configurationSetting.Value);
            if (!keyValueExists && throwExceptionIfEmpty) throw new ArgumentNullException(configurationSetting.Key);
            return keyValueExists ? configurationSetting.Value : defaultValue;
        }
    }

}
