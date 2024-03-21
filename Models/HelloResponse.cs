namespace hello_dotnet.Models;

public class HelloResponse
{
    public string? Message { get; init; }
    public string? ReqParam { get; init; }
    public Dictionary<string, string?>? ServerHeaders { get; init; }
}