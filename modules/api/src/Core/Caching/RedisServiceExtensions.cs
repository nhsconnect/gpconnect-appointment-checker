using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Core.Caching;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration,
        string environmentName)
    {
        var redisConnectionString = configuration.GetSection("Redis")["RedisConnectionString"]
                                    ?? throw new Exception("Missing Redis connection string");

        var options = ConfigurationOptions.Parse(redisConnectionString);
        options.Ssl = string.Equals(configuration.GetSection("Redis")["UseSsl"], "true",
            StringComparison.OrdinalIgnoreCase);

        var redis = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(redis);


        return services;
    }
}