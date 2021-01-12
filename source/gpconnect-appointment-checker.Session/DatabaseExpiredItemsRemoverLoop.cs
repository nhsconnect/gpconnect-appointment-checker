using gpconnect_appointment_checker.Caching.Interfaces;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Caching
{
    public sealed class DatabaseExpiredItemsRemoverLoop : IDatabaseExpiredItemsRemoverLoop
    {
        private static readonly TimeSpan MinimumExpiredItemsDeletionInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _expiredItemsDeletionInterval;
        private DateTimeOffset _lastExpirationScan;
        private readonly ICachingService _cachingService;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly DateTimeOffset _utcNow;

        public DatabaseExpiredItemsRemoverLoop(
            IOptions<PgSqlCacheOptions> options,
            ICachingService cachingService,
            IHostApplicationLifetime applicationLifetime)
        {
            var cacheOptions = options.Value;

            if (cacheOptions.ExpiredItemsDeletionInterval < MinimumExpiredItemsDeletionInterval)
            {
                throw new ArgumentException($"{nameof(PgSqlCacheOptions.ExpiredItemsDeletionInterval)} cannot be less the minimum value of {MinimumExpiredItemsDeletionInterval.TotalMinutes} minutes.");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            _cachingService = cachingService;
            _expiredItemsDeletionInterval = cacheOptions.ExpiredItemsDeletionInterval;
            _utcNow = DateTimeOffset.UtcNow;
        }

        public void Start()
        {
            Task.Run(DeleteExpiredCacheItems);
        }

        private void OnShutdown()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task DeleteExpiredCacheItems()
        {
            while (true)
            {
                if ((_utcNow - _lastExpirationScan) > _expiredItemsDeletionInterval)
                {
                    try
                    {
                        await _cachingService.DeleteExpiredCacheItemsAsync(_cancellationTokenSource.Token);
                        _lastExpirationScan = _utcNow;
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }

                try
                {
                    await Task
                            .Delay(_expiredItemsDeletionInterval, _cancellationTokenSource.Token)
                            .ConfigureAwait(true);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
