namespace hello_dotnet.Downstream;

public class ForwardForHttpClientHandler: DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string XForwardedForHeader = "x-forwarded-for";
    
    public ForwardForHttpClientHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(XForwardedForHeader, out var forwardIp) && forwardIp.Count == 1)
        {
            request.Headers.Add(XForwardedForHeader, forwardIp.First());
        }

        return base.SendAsync(request, cancellationToken);
    }
}