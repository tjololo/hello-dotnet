namespace hello_dotnet.Invokers;

public class RequestEventArgs : EventArgs
{
    public int Delay { get; set; }
    public string? Method { get; set; }
}