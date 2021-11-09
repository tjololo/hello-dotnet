using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis;

namespace hello_dotnet.Downstream
{


    public class RedisChacheDownstreamService : DownstreamBase, IDownstreamService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MemCacheDownStreamService> _logger;
        private readonly HttpClient _httpClient;
        private IRedisCacheClient _cache;
        public RedisChacheDownstreamService(ILogger<MemCacheDownStreamService> logger, IConfiguration configuration, HttpClient httpClient, IRedisCacheClient cache) : base(logger, httpClient)
        {
            _config = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _cache = cache;
        }
        public async Task<CacheResponse> GetAsyncDownstream(string name)
        {
            IRedisDatabase redis = _cache.GetDbFromConfiguration();
            string result = "", cacheStatus = "rediscache HIT for " + name;
            try
            {
                result = await redis.GetAsync<string>(name);
                if (result == null)
                {
                    result = await DoDownStreamCall();
                    await redis.AddAsync<string>(name, result, DateTimeOffset.Now.AddMinutes(1));
                    cacheStatus = "rediscache MISS for " + name;
                }
            }
            catch (RedisConnectionException e)
            {
                _logger.LogWarning("Failed to connect to redis, falling back to backend", e);
                result = await DoDownStreamCall();
                cacheStatus = "rediscache exception";
            }
            return new CacheResponse(result, cacheStatus);//"rediscache miss for " + name);
        }

        private async Task<string> DoDownStreamCall()
        {
            var enabled = _config["Downstream:Enabled"];
            if (_config["Downstream:Enabled"] == "True")
            {
                var downStream = _config["Downstream:URL"];
                return await DoDownstreamHttpCall(downStream);
            }
            else
            {
                await Task.Delay(3000);
                return "test";
            }
        }
    }
}