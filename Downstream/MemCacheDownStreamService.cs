using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

namespace hello_dotnet.Downstream
{


    public class MemCacheDownStreamService : DownstreamBase, IDownstreamService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MemCacheDownStreamService> _logger;
        private readonly HttpClient _httpClient;
        private IMemoryCache _cache;

        private readonly MemoryCacheEntryOptions memCacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(5))
        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
        public MemCacheDownStreamService(ILogger<MemCacheDownStreamService> logger, IConfiguration configuration, HttpClient httpClient, IMemoryCache cache) : base(logger, httpClient)
        {
            _config = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
        }
        public async Task<CacheResponse> GetAsyncDownstream(string name)
        {
            var enabled = _config["Downstream:Enabled"];
            if (_config["Downstream:Enabled"] == "True")
            {
                var downStream = _config["Downstream:URL"];
                var response = "";
                var cacheStatus = "memcache hit for " + name;
                if (!_cache.TryGetValue(name, out response)) {
                    response = await DoDownstreamHttpCall(downStream);
                    _cache.Set(name, response, memCacheOptions);
                    cacheStatus = "memcache miss for " + name;
                }
                return new CacheResponse(response, cacheStatus);
            }
            else
            {
                return await Task.FromResult(new CacheResponse("No downstream configured", "N/A"));
            }
        }
    }
}