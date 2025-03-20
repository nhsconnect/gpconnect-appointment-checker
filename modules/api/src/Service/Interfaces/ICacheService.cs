using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using StackExchange.Redis;

namespace gpconnect_appointment_checker.api.Service.Interfaces;

public interface ICacheService
{
    Task<T?> GetPageAsync<T>(string key, string field);

    Task<HashEntry[]> GetAllHashFieldsForHashSetAsync(string key);

    Task SetPageAsync<T>(string key, string field, T? value);

    Task<RedisValue> HashGetAsync(string key, string field);

    Task<bool> RemoveAsync(string key);

    IBatch CreateBatch();
}