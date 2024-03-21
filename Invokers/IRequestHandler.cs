namespace hello_dotnet.Invokers;

public interface IRequestHandler
{
    public Task OnPreRequest(string method, int delay);

    public Task OnPostRequest(string method, int delay);

    public void RegisterPreRequestEventReceiver(RequestHandler.AsyncEventHandler<RequestEventArgs> a);

    public void RegisterPostRequestEventReceiver(RequestHandler.AsyncEventHandler<RequestEventArgs> a);
}