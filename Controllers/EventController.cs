using System.Threading.Tasks;
using hello_dotnet.Invokers;
using Microsoft.AspNetCore.Mvc;

namespace hello_dotnet.Controllers
{
    [Route("events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRequestHandler requestHandler;

        public EventController(IRequestHandler requestHandler)
        {
            this.requestHandler = requestHandler;
        }

        // GET: events
        [HttpGet]
        public async Task<string> Get(int delay = 0)
        {
            await requestHandler.OnRequestReceived("Get", delay);
            return "Hello";
        }
    }
}