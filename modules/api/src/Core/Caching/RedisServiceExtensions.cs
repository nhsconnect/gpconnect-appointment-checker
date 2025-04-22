using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Core.Caching;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration,
        string environmentName)
    {
        var redisConnectionString = configuration.GetSection("Redis")["RedisConnectionString"]
                                    ?? throw new Exception("Missing Redis connection string");

        Console.WriteLine($"Redis connection string: {redisConnectionString}");

        var options = ConfigurationOptions.Parse(redisConnectionString);

        // Always force these for AWS Valkey Serverless -- change to false when running redis via docker / or configure TLS on docker
        options.Ssl = true;
        options.AbortOnConnectFail = false;

        var redis = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        return services;
    }
}