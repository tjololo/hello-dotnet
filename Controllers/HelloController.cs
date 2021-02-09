using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using hello_dotnet.Downstream;

namespace hello_dotnet.Controllers
{
    [ApiController]
    public class HelloController : ControllerBase
    {

        private readonly ILogger<HelloController> _logger;
        private readonly IDownstreamService _downstream;
        public HelloController(ILogger<HelloController> logger, IDownstreamService downstream)
        {
            _logger = logger;
            _downstream = downstream;
        }

        [HttpGet("hello")]
        public async Task<Hello> Get(int sleep = 0)
        {
            System.Threading.Thread.Sleep(sleep * 1000);
            var h = new Hello("Hello");
            return await Task.FromResult(h);
        }
        [HttpGet("calldown")]
        public async Task<ActionResult<HttpResponseMessage>> Downstream()
        {
            try {
                string downstreamResponse = await _downstream.GetAsyncDownstream();
                return await Task.FromResult(Ok(new Hello(downstreamResponse)));
            } catch (DownstreamConfigException e) {
                _logger.LogError("Configuration error with downstream", e);
                return await Task.FromResult(StatusCode(503, "Downstream call failed"));
            }
        }
    }
}
