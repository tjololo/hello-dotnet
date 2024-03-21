using Microsoft.AspNetCore.Http.Features;

namespace hello_dotnet.Factories
{
    public class TracingHttpContextFactory : IHttpContextFactory
    {
        private readonly DefaultHttpContextFactory _defaultFactory;

        public TracingHttpContextFactory(IServiceProvider serviceProvider)
        {
            _defaultFactory = new DefaultHttpContextFactory(serviceProvider);
        }

        public HttpContext Create(IFeatureCollection featureCollection)
        {
            var httpContext = _defaultFactory.Create(featureCollection);

            if (httpContext.Request.Headers.TryGetValue("X-B3-TraceId", out var traceId) &&
                httpContext.Request.Headers.TryGetValue("X-B3-SpanId", out var spanId) &&
                httpContext.Request.Headers.TryGetValue("X-B3-Sampled", out var sampled))
            {
                httpContext.Request.Headers.Append("traceparent", $"00-{traceId}:{spanId}-{(sampled == "1" ? "01" : "00")}");
            }

            return httpContext;
        }

        public void Dispose(HttpContext httpContext)
        {
            _defaultFactory.Dispose(httpContext);
        }
    }
}