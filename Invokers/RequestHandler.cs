using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hello_dotnet.Receivers;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Invokers;

public class RequestHandler : IRequestHandler
{
    public event AsyncEventHandler<RequestEventArgs>? RequestReceived;
    private readonly ILogger<RequestHandler> logger;
    
    public delegate Task AsyncEventHandler<T>(object? sender, T e);

    public RequestHandler(ILogger<RequestHandler> logger, IEnumerable<IEventReceiver> eventReceivers)
    {
        this.logger = logger;
        foreach (var eventReceiver in eventReceivers)
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

    public void RegisterRequestReceiverEventHandler(AsyncEventHandler<RequestEventArgs> a)
    {
        RequestReceived += a;
    }
}