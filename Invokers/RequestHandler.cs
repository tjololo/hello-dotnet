using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hello_dotnet.Receivers;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Invokers;

public class RequestHandler : IRequestHandler
{
    public event AsyncEventHandler<RequestEventArgs>? PreRequest;
    public event AsyncEventHandler<RequestEventArgs>? PostRequest;
    private readonly ILogger<RequestHandler> logger;
    public delegate Task AsyncEventHandler<T>(object? sender, T e);

    public RequestHandler(ILogger<RequestHandler> logger, IEnumerable<IEventReceiver> eventReceivers)
    {
        this.logger = logger;
        foreach (var eventReceiver in eventReceivers)
        {
            RegisterPreRequestEventReceiver(eventReceiver.PreRequest);
            RegisterPostRequestEventReceiver(eventReceiver.PostRequest);
        }
    }

    public async Task OnPreRequest(string method, int delay)
    {
        logger.LogInformation("Delegating process pre event for method {method} with dealy {delay}", method, delay);
        if (PreRequest is not null)
        {
            RequestEventArgs args = new RequestEventArgs() { Delay = delay, Method = method };
            await PreRequest.Invoke(this, args);
        }
    }
    
    public async Task OnPostRequest(string method, int delay)
    {
        logger.LogInformation("Delegating process post event for method {method} with dealy {delay}", method, delay);
        if (PostRequest is not null)
        {
            RequestEventArgs args = new RequestEventArgs() { Delay = delay, Method = method };
            await PostRequest.Invoke(this, args);
        }
    }

    public void RegisterPreRequestEventReceiver(AsyncEventHandler<RequestEventArgs> a)
    {
        PreRequest += a;
    }
    
    public void RegisterPostRequestEventReceiver(AsyncEventHandler<RequestEventArgs> a)
    {
        PostRequest += a;
    }
}