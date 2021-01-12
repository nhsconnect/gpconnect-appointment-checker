using Microsoft.Extensions.DependencyInjection;
using System;

namespace gpconnect_appointment_checker.Caching
{
    public static class PgSqlCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedPgSqlCache(this IServiceCollection services, Action<PgSqlCacheOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            services.AddOptions();
            services.Configure(setupAction);
            return services;
        }
    }
}
