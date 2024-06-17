using Microsoft.Extensions.Caching.Memory;

namespace GpConnect.AppointmentChecker.Api.Core.HttpClientServices;

public static class MemoryCacheExtensions
{
    private static IConfigurationSection _cacheConfig;

    public static void AddMemoryCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        _cacheConfig = configuration.GetSection("CacheConfig");
        services.AddMemoryCache(options => new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(_cacheConfig.GetValue<int>("TimeoutHours")))
            .SetPriority(CacheItemPriority.High)
        );
    }
}
