using Dapper;
using gpconnect_appointment_checker.DAL.Constants;
using gpconnect_appointment_checker.DAL.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Caching
{
    public class CachingService : ICachingService
    {
        private readonly ILogger<CachingService> _logger;
        private readonly IDataService _dataService;
        public const int CacheItemIdColumnWidth = 449;
        private readonly DateTimeOffset _utcNow;

        public CachingService(IConfiguration configuration, ILogger<CachingService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
            _utcNow = DateTimeOffset.UtcNow;
        }

        public async Task RefreshCacheItemAsync(string key, CancellationToken cancellationToken)
        {
            await GetCacheItemAsync(key, false, cancellationToken);
        }

        public void DeleteCacheItem(string key)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            _dataService.ExecuteFunction(Schemas.Caching, Functions.DeleteCacheItem, parameters);
        }

        public async Task DeleteCacheItemAsync(string key, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            await _dataService.ExecuteFunctionAsync(Schemas.Caching, Functions.DeleteCacheItem, parameters, cancellationToken);
        }

        public async Task SetCacheItemAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            var absoluteExpiration = GetAbsoluteExpiration(_utcNow, options);
            ValidateOptions(options.SlidingExpiration, absoluteExpiration);

            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            parameters.Add("_dist_cache_value", value, DbType.Binary, ParameterDirection.Input);

            if (options.SlidingExpiration.HasValue)
            {
                parameters.Add("_dist_cache_sliding_expiration_seconds", options.SlidingExpiration.Value.TotalSeconds, DbType.Double, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_dist_cache_sliding_expiration_seconds", DBNull.Value, DbType.Double, ParameterDirection.Input);
            }

            if (absoluteExpiration.HasValue)
            {
                parameters.Add("_dist_cache_absolute_expiration", absoluteExpiration.Value, DbType.DateTimeOffset, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_dist_cache_absolute_expiration", DBNull.Value, DbType.DateTimeOffset, ParameterDirection.Input);
            }

            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            await _dataService.ExecuteFunctionAsync(Schemas.Caching, Functions.SetCache, parameters, cancellationToken);
        }

        public void DeleteExpiredCacheItems()
        {
            var parameters = new DynamicParameters();
            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            _dataService.ExecuteFunction(Schemas.Caching, Functions.DeleteExpiredCacheItems, parameters);
        }

        public async Task DeleteExpiredCacheItemsAsync(CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            await _dataService.ExecuteFunctionAsync(Schemas.Caching, Functions.DeleteExpiredCacheItems, parameters, cancellationToken);
        }

        public virtual byte[] GetCacheItem(string key)
        {
            return GetCacheItem(key, true);
        }

        public virtual async Task<byte[]> GetCacheItemAsync(string key, CancellationToken cancellationToken)
        {
            return await GetCacheItemAsync(key, true, cancellationToken);
        }

        public virtual byte[] GetCacheItem(string key, bool includeValue)
        {
            byte[] value = null;

            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            _dataService.ExecuteFunction(Schemas.Caching, Functions.UpdateCacheItem, parameters);

            if (includeValue)
            {
                var result = _dataService.ExecuteFunction<DTO.Response.Caching.Item>(Schemas.Caching, Functions.GetCacheItem, parameters);
                value = result?.FirstOrDefault()?.Value;
            }

            return value;
        }

        public virtual async Task<byte[]> GetCacheItemAsync(string key, bool includeValue, CancellationToken cancellationToken)
        {
            byte[] value = null;

            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            await _dataService.ExecuteFunctionAsync(Schemas.Caching, Functions.UpdateCacheItem, parameters, cancellationToken);

            if (includeValue)
            {
                var result = await _dataService.ExecuteFunctionAsync<DTO.Response.Caching.Item>(Schemas.Caching, Functions.GetCacheItem, parameters, cancellationToken);
                value = result?.FirstOrDefault()?.Value;
            }
            return value;
        }

        public void RefreshCacheItem(string key)
        {
            GetCacheItem(key, false);
        }

        public void SetCacheItem(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var absoluteExpiration = GetAbsoluteExpiration(_utcNow, options);
            ValidateOptions(options.SlidingExpiration, absoluteExpiration);

            var parameters = new DynamicParameters();
            parameters.Add("_dist_cache_id", key, DbType.String, ParameterDirection.Input, CacheItemIdColumnWidth);
            parameters.Add("_dist_cache_value", value, DbType.Binary, ParameterDirection.Input);

            if (options.SlidingExpiration.HasValue)
            {
                parameters.Add("_dist_cache_sliding_expiration_seconds", options.SlidingExpiration.Value.TotalSeconds, DbType.Double, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_dist_cache_sliding_expiration_seconds", DBNull.Value, DbType.Double, ParameterDirection.Input);
            }

            if (absoluteExpiration.HasValue)
            {
                parameters.Add("_dist_cache_absolute_expiration", absoluteExpiration.Value, DbType.DateTimeOffset, ParameterDirection.Input);
            }
            else
            {
                parameters.Add("_dist_cache_absolute_expiration", DBNull.Value, DbType.DateTimeOffset, ParameterDirection.Input);
            }

            parameters.Add("_utc_now", _utcNow, DbType.DateTimeOffset, ParameterDirection.Input);
            _dataService.ExecuteFunction(Schemas.Caching, Functions.SetCache, parameters);
        }

        protected DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset utcNow, DistributedCacheEntryOptions options)
        {
            DateTimeOffset? absoluteExpiration = null;
            if (options.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = utcNow.Add(options.AbsoluteExpirationRelativeToNow.Value);
            }
            else if (options.AbsoluteExpiration.HasValue)
            {
                if (options.AbsoluteExpiration.Value <= utcNow)
                {
                    throw new InvalidOperationException("The absolute expiration value must be in the future.");
                }

                absoluteExpiration = options.AbsoluteExpiration.Value;
            }
            return absoluteExpiration;
        }

        protected void ValidateOptions(TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
        {
            if (!slidingExpiration.HasValue && !absoluteExpiration.HasValue)
            {
                throw new InvalidOperationException("Either absolute or sliding expiration needs to be provided.");
            }
        }
    }
}
