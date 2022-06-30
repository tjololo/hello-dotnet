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

    public async Task OnRequestProcessed(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Event processing for method {args.Method} in receiver 1");
        await Task.Delay(args.Delay);
        logger.LogInformation("Event processed in receiver 1");
    }
}