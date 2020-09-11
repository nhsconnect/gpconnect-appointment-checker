using Dapper;
using gpconnect_appointment_checker.DAL.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace gpconnect_appointment_checker.DAL
{
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        public ConfigurationSource Source { get; }

        public CustomConfigurationProvider(ConfigurationSource source)
        {
            Source = source;
        }

        public override async void Load()
        {
            await using var conn = new NpgsqlConnection(Source.Configuration);
            var results = await conn.QueryAsync<ConfigurationEntries>(Source.Query);

            foreach(var result in results)
            {
                Set(result.Key, result.Value);
            }
        }
    }
}
