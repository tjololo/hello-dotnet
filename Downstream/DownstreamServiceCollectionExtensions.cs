using Microsoft.Extensions.DependencyInjection.Extensions;

namespace hello_dotnet.Downstream;

public static  class DownstreamServiceCollectionExtensions
{
    public static void AddDownstreamService<T>(this IServiceCollection services) where T : class, IDownstreamService
    {
        services.TryAddTransient<ForwardForHttpClientHandler>();
        services.AddHttpClient<IDownstreamService, T>()
            .AddHttpMessageHandler<ForwardForHttpClientHandler>();
    }
    
    public static void AddDownstreamServiceWithCustomHandler<T, TV>(this IServiceCollection services) where T: class, IDownstreamService where TV: DelegatingHandler
    {
        services.TryAddTransient<TV>();
        services.AddHttpClient<IDownstreamService, T>()
            .AddHttpMessageHandler<TV>();
    }
}