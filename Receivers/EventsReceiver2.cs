using hello_dotnet.Invokers;

namespace hello_dotnet.Receivers;

public class EventsReceiver2 : IEventReceiver
{
    private readonly ILogger<EventsReceiver2> _logger;
    public EventsReceiver2(ILogger<EventsReceiver2> logger)
    {
        _logger = logger;
    }

    public async Task PreRequest(object? sender, RequestEventArgs args)
    {
        _logger.LogInformation($"Pre Event processing for method {args.Method} in receiver 2");
        await Task.Delay(args.Delay);
        _logger.LogInformation("Pre Event processed in receiver 2");
    }

    public async Task PostRequest(object? sender, RequestEventArgs args)
    {
        _logger.LogInformation($"Post Event processing for method {args.Method} in receiver 2");
        await Task.Delay(args.Delay);
        _logger.LogInformation("Post Event processed in receiver 2");
    }
}