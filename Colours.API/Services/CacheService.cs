
using StackExchange.Redis;
using System.Text.Json;

namespace Colours.API.Services;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;

    public CacheService(IConfiguration config)
    {
        var redisHost = config["Redis:Host"];
        var redisPort = config["Redis:Port"];
        var redis = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
        _cacheDb = redis.GetDatabase();
    }

    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);
        if (!string.IsNullOrWhiteSpace(value))
            return JsonSerializer.Deserialize<T>(value);

        return default;
    }

    public object RemoveData(string key)
    {
        if(_cacheDb.KeyExists(key))
            return _cacheDb.KeyDelete(key);
        return false;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
        return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
    }

}
