using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface ICachingService
    {
        byte[] GetCacheItem(string key);

        Task<byte[]> GetCacheItemAsync(string key, CancellationToken cancellationToken);

        byte[] GetCacheItem(string key, bool includeValue);

        Task<byte[]> GetCacheItemAsync(string key, bool includeValue, CancellationToken cancellationToken);

        void RefreshCacheItem(string key);

        Task RefreshCacheItemAsync(string key, CancellationToken cancellationToken);

        void DeleteCacheItem(string key);

        Task DeleteCacheItemAsync(string key, CancellationToken cancellationToken);

        void SetCacheItem(string key, byte[] value, DistributedCacheEntryOptions options);

        Task SetCacheItemAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken);

        void DeleteExpiredCacheItems();

        Task DeleteExpiredCacheItemsAsync(CancellationToken cancellationToken);
    }
}
