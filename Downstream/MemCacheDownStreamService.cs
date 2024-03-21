using Microsoft.Extensions.Caching.Memory;

namespace hello_dotnet.Downstream;

public class MemCacheDownStreamService : DownstreamBase, IDownstreamService
{
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    private readonly MemoryCacheEntryOptions _memCacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(5))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

    public MemCacheDownStreamService(ILogger<MemCacheDownStreamService> logger, IConfiguration configuration,
        HttpClient httpClient, IMemoryCache cache) : base(logger, httpClient)
    {
        _config = configuration;
        _cache = cache;
    }

    public async Task<CacheResponse> GetAsyncDownstream(string name)
    {
        string? downStreamUrl = _config["Downstream:URL"];
        if (_config["Downstream:Enabled"] != "True" || downStreamUrl == null)
        {
            return await Task.FromResult(new CacheResponse("No downstream configured", "N/A"));
        }

        var cacheStatus = "memcache hit for " + name;
        if (_cache.TryGetValue(name, out string? response) && response != null)
        {
            return new CacheResponse(response, cacheStatus);
        }

        response = await DoDownstreamHttpCall(downStreamUrl);
        _cache.Set(name, response, _memCacheOptions);
        cacheStatus = "memcache miss for " + name;
        return new CacheResponse(response, cacheStatus);
    }
}