using System.Threading.Tasks;

namespace hello_dotnet.Events;

public interface IRequestHandler
{
    public Task OnRequestReceived(string method, int delay);
    public Task RegisterHandlers();
}