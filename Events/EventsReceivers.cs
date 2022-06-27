using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Events;

public class EventsReceivers: IEventReceiver
{
    private readonly ILogger<EventsReceivers> logger;
    public EventsReceivers(ILogger<EventsReceivers> logger)
    {
        this.logger = logger;
    }

    public async Task OnRequestProcessed(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Event processing for method {args.Method}");
        await Task.Delay(args.Delay);
        logger.LogInformation("Event processed");
    }
}