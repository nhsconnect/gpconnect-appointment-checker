using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Configuration.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;

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
            MappingExtensions.ConfigureMappingServices();

            LoadConfiguration<Spine>("SELECT * FROM configuration.get_spine_configuration()", "Spine");
            LoadConfiguration<General>("SELECT * FROM configuration.get_general_configuration()", "General");
            LoadConfiguration<Sso>("SELECT * FROM configuration.get_sso_configuration()", "SingleSignOn");
            LoadConfiguration<Email>("SELECT * FROM configuration.get_email_configuration()", "Email");
        }

        private void LoadConfiguration<T>(string query, string configurationPrefix) where T : class
        {
            //using NpgsqlConnection connection = new NpgsqlConnection(Source.ConnectionString);
            //var result = connection.QueryFirstOrDefault<T>(query);
            //var json = JsonConvert.SerializeObject(result);
            //var configuration = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            //foreach (var configEntry in configuration)
            //{
            //    Set($"{configurationPrefix}:{configEntry.Key}", configEntry.Value ?? string.Empty);
            //}
        }
    }
}
