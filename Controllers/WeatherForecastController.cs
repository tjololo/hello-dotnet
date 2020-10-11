using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace hello_dotnet.Controllers
{
    [ApiController]
    public class HelloWorldController : ControllerBase
    {

        private readonly ILogger<HelloWorldController> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        [Obsolete]
        public HelloWorldController(ILogger<HelloWorldController> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        [HttpGet("hello")]
        public async Task<Hello> Get(int sleep = 0)
        {
            System.Threading.Thread.Sleep(sleep*1000);
            var h = new Hello("Hello");
            return await Task.FromResult(h);
        }
    }
}
