using hello_dotnet.Invokers;
using Microsoft.AspNetCore.Mvc;

namespace hello_dotnet.Controllers
{
    [Route("events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRequestHandler _requestHandler;
        private readonly ILogger<EventController> _logger;

        public EventController(IRequestHandler requestHandler, ILogger<EventController> logger)
        {
            this._requestHandler = requestHandler;
            this._logger = logger;
        }

        // GET: events
        [HttpGet]
        public async Task<string> Get(int delay = 0)
        {
            await _requestHandler.OnPreRequest("Get", delay);
            _logger.LogInformation("Processing request in controller");
            await _requestHandler.OnPostRequest("Get", delay);
            return "Hello";
        }
    }
}