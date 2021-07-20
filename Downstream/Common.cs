using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace hello_dotnet.Downstream
{
    public class DownstreamBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public DownstreamBase(ILogger logger, HttpClient httpClient) {
            _logger = logger;
            _httpClient = httpClient;
        }

        protected async Task<string> DoDownstreamHttpCall(string url) {
                if (url == null && url != "")
                {
                    _logger.LogError("Downstream enabled but not URL set for downstream");
                    throw new DownstreamConfigException("Missing url for downstream");
                }
                try
                {
                    HttpResponseMessage resp = await _httpClient.GetAsync(url);
                    resp.EnsureSuccessStatusCode();
                    return await resp.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    throw new DownstreamConfigException("downstream call returned exception", e);
                }
            }
    }

    public interface IDownstreamService
    {
        Task<CacheResponse> GetAsyncDownstream(string name);
    }
    
    public class DownstreamConfigException : Exception
    {
        public DownstreamConfigException() { }

        public DownstreamConfigException(string message) : base(message) { }

        public DownstreamConfigException(string message, Exception inner) : base(message, inner) { }
    }
}