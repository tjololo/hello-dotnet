using System.Threading.Tasks;
using hello_dotnet.Invokers;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Receivers;

public class EventsReceiver2: IEventReceiver
{
    private readonly ILogger<EventsReceiver2> logger;
    public EventsReceiver2(ILogger<EventsReceiver2> logger)
    {
        this.logger = logger;
    }

    public async Task PreRequest(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Pre Event processing for method {args.Method} in receiver 2");
        await Task.Delay(args.Delay);
        logger.LogInformation("Pre Event processed in receiver 2");
    }
    
    public async Task PostRequest(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Post Event processing for method {args.Method} in receiver 2");
        await Task.Delay(args.Delay);
        logger.LogInformation("Post Event processed in receiver 2");
    }
}