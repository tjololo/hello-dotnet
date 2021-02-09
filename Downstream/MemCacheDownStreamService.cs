using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;

namespace hello_dotnet.Downstream
{


    public class MemCacheDownStreamService : DownstreamBase, IDownstreamService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MemCacheDownStreamService> _logger;
        private readonly HttpClient _httpClient;
        public MemCacheDownStreamService(ILogger<MemCacheDownStreamService> logger, IConfiguration configuration, HttpClient httpClient) : base(logger, httpClient)
        {
            _config = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<string> GetAsyncDownstream()
        {
            var enabled = _config["Downstream:Enabled"];
            if (_config["Downstream:Enabled"] == "True")
            {
                var downStream = _config["Downstream:URL"];
                return await DoDownstreamHttpCall(downStream);
            }
            else
            {
                return await Task.FromResult("No downstream configured");
            }
        }
    }
}