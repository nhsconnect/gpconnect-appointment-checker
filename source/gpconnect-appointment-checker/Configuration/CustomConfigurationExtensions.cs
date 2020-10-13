using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Configuration;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;

namespace gpconnect_appointment_checker.Configuration
{
    public static class CustomConfigurationExtensions
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder configuration, Action<ConfigurationOptions> options)
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
        public string Query { get; set; }

        public ConfigurationSource(ConfigurationOptions options)
        {
            ConnectionString = options.ConnectionString;
            Query = options.Query;
            
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomConfigurationProvider(this);
        }
    }

    public class ConfigurationOptions
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
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
            using NpgsqlConnection connection = new NpgsqlConnection(Source.ConnectionString);
            var results = connection.Query<Spine>(Source.Query);
            var json = JsonConvert.SerializeObject(results);
            var spineConfiguration = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            foreach (var spineConfig in spineConfiguration)
            {
                //Set(spineConfig.Key, spineConfig.Value);
            }
        }
    }
}
