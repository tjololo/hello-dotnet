using System.Threading.Tasks;
using hello_dotnet.Invokers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Controllers
{
    [Route("events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRequestHandler requestHandler;
        private readonly ILogger<EventController> logger;

        public EventController(IRequestHandler requestHandler, ILogger<EventController> logger)
        {
            this.requestHandler = requestHandler;
            this.logger = logger;
        }

        // GET: events
        [HttpGet]
        public async Task<string> Get(int delay = 0)
        {
            await requestHandler.OnPreRequest("Get", delay);
            logger.LogInformation("Processing request in controller");
            await requestHandler.OnPostRequest("Get", delay);
            return "Hello";
        }
    }
}