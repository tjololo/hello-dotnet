using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace hello_dotnet.Events;

public class EventsReceivers
{

    public static async Task onRequestProcessed(object? sender, RequestEventArgs args)
    {
        Console.WriteLine($"Event processing for method {args.Method}");
        await Task.Delay(args.Delay);
        Console.WriteLine("Event processed");
    }
}