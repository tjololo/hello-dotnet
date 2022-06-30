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

    public async Task OnRequestProcessed(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Event processing for method {args.Method} in receiver 2");
        await Task.Delay(args.Delay);
        logger.LogInformation("Event processed in receiver 2");
    }
}