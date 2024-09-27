using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using hello_dotnet.Downstream;
using hello_dotnet.Models;
using hello_dotnet.Utils;

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
        public async Task<HelloResponse> Get(int sleep = 0, string name = "")
        {
            await Task.Delay(sleep * 1000);
            var headers = new Dictionary<string, string?>();
            var greeting = "Hello";
            if (name.Equals("oslo"))
            {
                greeting = $"Hello from Oslo the time is {GetOsloTime()}";
            }
            foreach (var header in Request.Headers)
            {
                headers.Add(header.Key, header.Value);
            }

            var resp = new HelloResponse()
            {
                Message = greeting,
                ReqParam = name,
                ServerHeaders = headers
            };
            return await Task.FromResult(resp);
        }

        [HttpGet("calldown")]
        public async Task<ActionResult<HttpResponseMessage>> Downstream(string name = "")
        {
            try
            {
                CacheResponse downstreamResponse = await _downstream.GetAsyncDownstream(name);
                return await Task.FromResult(Ok(downstreamResponse));
            }
            catch (DownstreamConfigException e)
            {
                _logger.LogError(e, "Configuration error with downstream");
                return await Task.FromResult(StatusCode(503, "Downstream call failed"));
            }
        }

        /// <summary>
        /// This is a test endpoint that generates cpu load
        /// </summary>
        /// <param name="time">Time in seconds to generate cpu load</param>
        /// <param name="load">Target cpu load to generate</param>
        /// <returns>A string with number of seconds of cpu load</returns>
        [HttpGet("cpu")]
        public async Task<string> LoadCpu(int time = 1, int load = 10)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            LoadGenerator.ConsumeCpu(load, time);
            watch.Stop();
            return await Task.FromResult(string.Join(" ", "CPU load for", watch.ElapsedMilliseconds / 1000, "seconds").Trim());
        }

        private static string GetOsloTime()
        {
            TimeZoneInfo norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
            DateTime norwegianNow = TimeZoneInfo.ConvertTime(DateTime.Now, norwegianTimeZone);

            string dateGenerated = norwegianNow.ToString("dd.MM.yyyy HH:mm:ss",CultureInfo.InvariantCulture);
            return dateGenerated;
        }
    }
}
