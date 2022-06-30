using System.Threading.Tasks;
using hello_dotnet.Invokers;

namespace hello_dotnet.Receivers;

public interface IEventReceiver
{
    public Task OnRequestProcessed(object? sender, RequestEventArgs args);
    
}