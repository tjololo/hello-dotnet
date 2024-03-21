namespace hello_dotnet.Downstream;

public class SimpleDownstreamService : DownstreamBase, IDownstreamService
{
    private readonly IConfiguration _config;
    public SimpleDownstreamService(ILogger<SimpleDownstreamService> logger, IConfiguration configuration, HttpClient httpClient) : base(logger, httpClient)
    {
        _config = configuration;
    }
    public async Task<CacheResponse> GetAsyncDownstream(string name)
    {
        var downStream = _config["Downstream:URL"];
        if (_config["Downstream:Enabled"] == "True" && downStream != null)
        {
            return new CacheResponse(await DoDownstreamHttpCall(downStream), "no cache");
        }

        return await Task.FromResult(new CacheResponse("No downstream configured", "N/A"));
    }
}