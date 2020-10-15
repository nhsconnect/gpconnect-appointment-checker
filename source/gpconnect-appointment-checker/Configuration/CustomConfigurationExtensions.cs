using Dapper;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace gpconnect_appointment_checker.Configuration
{
    public static class CustomConfigurationExtensions
    {
        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder configuration, Action<ConfigurationOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var myConfigurationOptions = new ConfigurationOptions();
            options(myConfigurationOptions);
            configuration.Add(new ConfigurationSource(myConfigurationOptions));
            return configuration;
        }

        public static string GetConfigurationString(this IConfigurationSection configurationSetting, string defaultValue = "")
        {
            var keyValueExists = configurationSetting.Exists() && !string.IsNullOrEmpty(configurationSetting.Value);
            return keyValueExists ? configurationSetting.Value : defaultValue;
        }
    }

    public class ConfigurationSource : IConfigurationSource
    {
        public string ConnectionString { get; set; }

        public ConfigurationSource(ConfigurationOptions options)
        {
            ConnectionString = options.ConnectionString;
            
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomConfigurationProvider(this);
        }
    }

    public class ConfigurationOptions
    {
        public string ConnectionString { get; set; }
    }

    public class CustomConfigurationProvider : ConfigurationProvider
    {
        public ConfigurationSource Source { get; }

        public CustomConfigurationProvider(ConfigurationSource source)
        {
            Source = source;
        }

        public override void Load()
        {
            LoadConfiguration<Spine>("SELECT * FROM configuration.spine", "Spine");
            LoadConfiguration<General>("SELECT * FROM configuration.general", "General");
            LoadConfiguration<Sso>("SELECT * FROM configuration.sso", "SingleSignOn");
        }

        private void LoadConfiguration<T>(string query, string configurationPrefix) where T : class
        {
            using NpgsqlConnection connection = new NpgsqlConnection(Source.ConnectionString);
            var result = connection.Query<T>(query).FirstOrDefault();
            var json = JsonConvert.SerializeObject(result);
            var configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            foreach (var configEntry in configuration)
            {
                Set($"{configurationPrefix}:{configEntry.Key}", configEntry.Value ?? string.Empty);
            }
        }
    }
}
