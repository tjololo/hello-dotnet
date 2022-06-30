using System.Threading.Tasks;

namespace hello_dotnet.Invokers;

public interface IRequestHandler
{
    public Task OnRequestReceived(string method, int delay);

    public void RegisterRequestReceiverEventHandler(RequestHandler.AsyncEventHandler<RequestEventArgs> a);
}