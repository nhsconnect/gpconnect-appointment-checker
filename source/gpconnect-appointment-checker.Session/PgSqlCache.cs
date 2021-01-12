using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Caching
{
    public class PgSqlCache : IDistributedCache
    {
        private static readonly TimeSpan MinimumExpiredItemsDeletionInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _expiredItemsDeletionInterval;
        private DateTimeOffset _lastExpirationScan;
        private readonly Action _deleteExpiredCachedItemsDelegate;
        private readonly TimeSpan _defaultSlidingExpiration;
        private readonly ICachingService _cachingService;
        private readonly DateTimeOffset _utcNow;

        public PgSqlCache(IOptions<PgSqlCacheOptions> options, ICachingService cachingService)
        {
            var cacheOptions = options.Value;

            if (cacheOptions.ExpiredItemsDeletionInterval < MinimumExpiredItemsDeletionInterval)
            {
                throw new ArgumentException($"{nameof(PgSqlCacheOptions.ExpiredItemsDeletionInterval)} cannot be less the minimum value of {MinimumExpiredItemsDeletionInterval.TotalMinutes} minutes.");
            }

            if (cacheOptions.DefaultSlidingExpiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheOptions.DefaultSlidingExpiration), cacheOptions.DefaultSlidingExpiration, "The sliding expiration value must be positive.");
            }

            _cachingService = cachingService;
            _expiredItemsDeletionInterval = cacheOptions.ExpiredItemsDeletionInterval;
            _deleteExpiredCachedItemsDelegate = _cachingService.DeleteExpiredCacheItems;
            _defaultSlidingExpiration = cacheOptions.DefaultSlidingExpiration;
            _utcNow = DateTimeOffset.UtcNow;
        }

        public byte[] Get(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var value = _cachingService.GetCacheItem(key);
            ScanForExpiredItemsIfRequired();
            return value;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var value = await _cachingService.GetCacheItemAsync(key, token);
            ScanForExpiredItemsIfRequired();
            return value;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (options == null) throw new ArgumentNullException(nameof(options));
            GetOptions(ref options);
            _cachingService.SetCacheItem(key, value, options);
            ScanForExpiredItemsIfRequired();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = new CancellationToken())
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (options == null) throw new ArgumentNullException(nameof(options));
            GetOptions(ref options);
            await _cachingService.SetCacheItemAsync(key, value, options, token);
            ScanForExpiredItemsIfRequired();
        }

        public void Refresh(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            _cachingService.RefreshCacheItem(key);
            ScanForExpiredItemsIfRequired();
        }

        public async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            await _cachingService.RefreshCacheItemAsync(key, token);
            ScanForExpiredItemsIfRequired();
        }

        public void Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _cachingService.DeleteCacheItem(key);
            ScanForExpiredItemsIfRequired();
        }

        public async Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            await _cachingService.DeleteCacheItemAsync(key, token);
            ScanForExpiredItemsIfRequired();
        }

        private void ScanForExpiredItemsIfRequired()
        {
            if ((_utcNow - _lastExpirationScan) > _expiredItemsDeletionInterval)
            {
                _lastExpirationScan = _utcNow;
                Task.Run(_deleteExpiredCachedItemsDelegate);
            }
        }

        private void GetOptions(ref DistributedCacheEntryOptions options)
        {
            if (!options.AbsoluteExpiration.HasValue && !options.AbsoluteExpirationRelativeToNow.HasValue && !options.SlidingExpiration.HasValue)
            {
                options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = _defaultSlidingExpiration
                };
            }
        }
    }
}
