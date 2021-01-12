using Dapper;
using gpconnect_appointment_checker.DAL.Constants;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;

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
            LoadConfiguration<Spine>("Spine", Functions.GetSpineConfiguration);
            LoadConfiguration<General>("General", Functions.GetGeneralConfiguration);
            LoadConfiguration<Sso>("SingleSignOn", Functions.GetSingleSignOnConfiguration);
        }

        private void LoadConfiguration<T>(string configurationPrefix, string functionName) where T : class
        {
            using NpgsqlConnection connection = new NpgsqlConnection(Source.ConnectionString);
            var result = connection.QueryFirstOrDefault<T>($"SELECT * FROM {Schemas.Configuration}.{functionName}()");
            var json = JsonConvert.SerializeObject(result);
            var configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            foreach (var configEntry in configuration)
            {
                Set($"{configurationPrefix}:{configEntry.Key}", configEntry.Value ?? string.Empty);
            }
        }
    }
}
