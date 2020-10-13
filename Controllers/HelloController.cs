using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace hello_dotnet.Controllers
{
    [ApiController]
    public class HelloController : ControllerBase
    {

        private readonly ILogger<HelloController> _logger;
        public HelloController(ILogger<HelloController> logger)
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
