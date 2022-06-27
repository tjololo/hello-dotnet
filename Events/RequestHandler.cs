using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Events;

public class RequestHandler : IRequestHandler
{
    public event AsyncEventHandler<RequestEventArgs>? RequestReceived;
    private readonly ILogger<RequestHandler> logger;
    
    public delegate Task AsyncEventHandler<T>(object? sender, T e);

    public RequestHandler(ILogger<RequestHandler> logger, IEventReceiver? eventReceiver)
    {
        this.logger = logger;
        if (eventReceiver is not null)
        {
            RegisterRequestReceiverEventHandler(eventReceiver.OnRequestProcessed);
        }
    }

    public async Task OnRequestReceived(string method, int delay)
    {
        logger.LogInformation("Delegating process event for method {method} with dealy {delay}", method, delay);
        if (RequestReceived is not null)
        {
            RequestEventArgs args = new RequestEventArgs() { Delay = delay, Method = method };
            await RequestReceived.Invoke(this, args);
        }
    }

    public async Task onRequestProcessed(object? sender, RequestEventArgs args)
    {
        logger.LogInformation($"Event processing for method {args.Method}");
        await Task.Delay(args.Delay);
        logger.LogInformation("Event processed 1111111");
    }

    public async Task RegisterHandlers()
    {
        RequestReceived += this.onRequestProcessed;
    }

    public void RegisterRequestReceiverEventHandler(AsyncEventHandler<RequestEventArgs> a)
    {
        RequestReceived += a;
    }
}