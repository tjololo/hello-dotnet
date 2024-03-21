using Microsoft.Extensions.Caching.Distributed;

namespace hello_dotnet.Downstream;

public class RedisChacheDownstreamService : DownstreamBase, IDownstreamService
{
    private readonly IConfiguration _config;
    private readonly IDistributedCache _cache;

    public RedisChacheDownstreamService(ILogger<MemCacheDownStreamService> logger, IConfiguration configuration,
        HttpClient httpClient, IDistributedCache cache) : base(logger, httpClient)
    {
        _config = configuration;
        _cache = cache;
    }

    public async Task<CacheResponse> GetAsyncDownstream(string name)
    {
        string cacheStatus = $"rediscache HIT for {name}";
        var result = await _cache.GetStringAsync(name);
        if (result == null)
        {
            result = await DoDownStreamCall();
            await _cache.SetStringAsync(name, result);
            cacheStatus = $"rediscache MISS for {name}";
        }

        return new CacheResponse(result, cacheStatus);
    }

    private async Task<string> DoDownStreamCall()
    {
        var downStream = _config["Downstream:URL"];
        if (_config["Downstream:Enabled"] == "True" && downStream != null)
        {
            return await DoDownstreamHttpCall(downStream);
        }

        await Task.Delay(3000);
        return "test";
    }
}