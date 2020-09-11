using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder GetCustomConfiguration(this IConfigurationBuilder configuration, Action<ConfigurationOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var myConfigurationOptions = new ConfigurationOptions();
            options(myConfigurationOptions);
            configuration.Add(new ConfigurationSource(myConfigurationOptions));
            return configuration;
        }
    }
}
