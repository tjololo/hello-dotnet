using System.Threading.Tasks;
using hello_dotnet.Invokers;

namespace hello_dotnet.Receivers;

public interface IEventReceiver
{
    public Task PreRequest(object? sender, RequestEventArgs args);
    
    public Task PostRequest(object? sender, RequestEventArgs args);
    
}