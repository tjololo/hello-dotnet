using System.Threading.Tasks;
using hello_dotnet.Invokers;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Receivers;

public class EventsReceiver1: IEventReceiver
{
    private readonly ILogger<EventsReceiver1> logger;
    public EventsReceiver1(ILogger<EventsReceiver1> logger)
    {
        this.logger = logger;
    }

    public async Task PreRequest(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Pre Event processing for method {args.Method} in receiver 1");
        await Task.Delay(args.Delay);
        logger.LogInformation("Pre Event processed in receiver 1");
    }
    
    public async Task PostRequest(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Post Event processing for method {args.Method} in receiver 1");
        await Task.Delay(args.Delay);
        logger.LogInformation("Post Event processed in receiver 1");
    }
}