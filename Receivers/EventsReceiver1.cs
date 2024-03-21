using hello_dotnet.Invokers;

namespace hello_dotnet.Receivers;

public class EventsReceiver1 : IEventReceiver
{
    private readonly ILogger<EventsReceiver1> _logger;
    public EventsReceiver1(ILogger<EventsReceiver1> logger)
    {
        _logger = logger;
    }

    public async Task PreRequest(object? sender, RequestEventArgs args)
    {
        _logger.LogInformation("Pre Event processing for method {Method} in receiver 1", args.Method);
        await Task.Delay(args.Delay);
        _logger.LogInformation("Pre Event processed in receiver 1");
    }

    public async Task PostRequest(object? sender, RequestEventArgs args)
    {
        _logger.LogInformation("Post Event processing for method {Method} in receiver 1", args.Method);
        await Task.Delay(args.Delay);
        _logger.LogInformation("Post Event processed in receiver 1");
    }
}