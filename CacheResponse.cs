namespace hello_dotnet
{
    public class CacheResponse
    {
        public CacheResponse(string message, string cache)
        {
            Message = message;
            Cache = cache;
        }
        public string Message { get; }
        public string Cache { get; }
    }
}