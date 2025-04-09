using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Core.Caching;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration,
        string environmentName)
    {
        var redisConnectionString = configuration.GetSection("Redis")["RedisConnectionString"]
                                    ?? throw new Exception("Missing Redis connection string");

        var useSslSetting = configuration.GetSection("Redis")["UseSsl"];
        var useSsl = string.Equals(useSslSetting, "true", StringComparison.OrdinalIgnoreCase);

        var options = new ConfigurationOptions
        {
            EndPoints = { redisConnectionString },
            Ssl = useSsl
        };

        var redis = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        return services;
    }
}