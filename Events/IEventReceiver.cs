using System.Threading.Tasks;

namespace hello_dotnet.Events;

public interface IEventReceiver
{
    public Task OnRequestProcessed(object? sender, RequestEventArgs args);
    
}