using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace hello_dotnet.Controllers
{
    [ApiController]
    public class HelloWorldController : ControllerBase
    {

        private readonly ILogger<HelloWorldController> _logger;
        public HelloWorldController(ILogger<HelloWorldController> logger)
        {
            _logger = logger;
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
