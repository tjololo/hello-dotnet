using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;

namespace hello_dotnet.Downstream
{

    public interface IDownstreamService
    {
        Task<string> GetAsyncDownstream();
    }

    public class DownstreamHttpService: IDownstreamService  
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DownstreamHttpService> _logger;
        private readonly HttpClient _httpClient;
        public DownstreamHttpService(ILogger<DownstreamHttpService> logger, IConfiguration configuration, HttpClient httpClient) {
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
                if (downStream == null && downStream != "")
                {
                    _logger.LogError("Downstream enabled but not URL set for downstream");
                    throw new DownstreamConfigException("Missing url for downstream");
                }
                try
                {
                    HttpResponseMessage resp = await _httpClient.GetAsync(downStream);
                    resp.EnsureSuccessStatusCode();
                    return await Task.FromResult($"Response from downstream: '{resp.Content.ReadAsStringAsync().Result}'");
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError("Failed to complete downstream request", e);
                    throw new DownstreamConfigException("downstream call returned exception", e);
                }
            }
            else
            {
                return await Task.FromResult("No downstream configured");
            }
        }
    }

    public class DownstreamConfigException : Exception
    {
        public DownstreamConfigException(){}

        public DownstreamConfigException(string message): base(message) {}

        public DownstreamConfigException(string message, Exception inner): base(message, inner) {}
    }
}