using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace hello_dotnet.Downstream
{
    public class SimpleDownstreamService : DownstreamBase, IDownstreamService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SimpleDownstreamService> _logger;
        private readonly HttpClient _httpClient;
        public SimpleDownstreamService(ILogger<SimpleDownstreamService> logger, IConfiguration configuration, HttpClient httpClient) : base(logger, httpClient)
        {
            _config = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<CacheResponse> GetAsyncDownstream(string name)
        {
            var enabled = _config["Downstream:Enabled"];
            if (_config["Downstream:Enabled"] == "True")
            {
                var downStream = _config["Downstream:URL"];
                return new CacheResponse(await DoDownstreamHttpCall(downStream), "no cache");
            }
            else
            {
                return await Task.FromResult(new CacheResponse("No downstream configured", "N/A"));
            }
        }
    }
}