using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Core.Caching;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration,
        string environmentName)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            string connectionString = configuration.GetSection("Redis")["RedisConnectionString"]
                ?? throw new Exception("Missing Redis connection string");
            Console.WriteLine($"Cache connection string: {connectionString}");

            var useSslSetting = configuration.GetSection("Redis")["UseSsl"];
            var useSsl = string.Equals(useSslSetting, "true", StringComparison.OrdinalIgnoreCase);

            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = true; 
            options.Ssl = useSsl;

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}
