using System.Text.Json;
using gpconnect_appointment_checker.api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Service;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(15);
    private readonly ILogger<RedisCacheService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _cache = redis.GetDatabase();
        _logger = logger;
        _redis = redis;
    }

    public async Task<(bool WasFound, T? Value)> TryGetAsync<T>(string key)
    {
        try
        {
            var value = await _cache.StringGetAsync(key);
            return !value.HasValue ? (false, default) : (true, JsonSerializer.Deserialize<T>(value!, JsonOptions));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Redis TryGet failed: {ex.Message}");
            return (false, default);
        }
    }

    public async Task<RedisValue> HashGetAsync(string key, string field)
    {
        return await _cache.HashGetAsync(key, field);
    }

    public async Task<HashEntry[]> GetAllHashFieldsForHashSetAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.HashGetAllAsync(key);
    }

    public async Task SetPageAsync<T>(string key, string field, T? value)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _cache.HashSetAsync(key, field, jsonData);
    }

    public async Task<T?> GetPageAsync<T>(string key, string field)
    {
        var value = await _cache.HashGetAsync(key, field);

        if (!value.HasValue)
            return default;

        if (typeof(T) == typeof(int))
        {
            return (T)(object)int.Parse(value!);
        }
        else if (typeof(T) == typeof(long))
        {
            return (T)(object)long.Parse(value!);
        }
        else if (typeof(T) == typeof(double))
        {
            return (T)(object)double.Parse(value!);
        }

        // Otherwise, assume it's JSON
        return JsonSerializer.Deserialize<T>(value!);
    }


    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            return await _cache.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis Remove failed: {ex.Message}");
            return false;
        }
    }
    
    public IBatch CreateBatch()
    {
        return _cache.CreateBatch();
    }
}