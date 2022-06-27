using System;

namespace hello_dotnet.Events;

public class RequestEventArgs: EventArgs
{
    public int Delay { get; set; }
    public string Method { get; set; }
}