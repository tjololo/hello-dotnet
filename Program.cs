using hello_dotnet.Downstream;
using hello_dotnet.Factories;
using hello_dotnet.Invokers;
using hello_dotnet.Receivers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

if ("true".Equals(builder.Configuration["Otel:Enabled"]))
{
    builder.Logging.AddOpenTelemetry();
}
ConfigureApp(builder.Configuration);
ConfigureService(builder.Services, builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014

app.Run();

void ConfigureApp(ConfigurationManager config)
{
    config.AddJsonFile("/config/appsettings.json", optional: true, reloadOnChange: true);
    string? configBasePathEnv = Environment.GetEnvironmentVariable("CONFIG_PATH");
    string configPath = $"{configBasePathEnv}/appsettings.json";
    config.AddJsonFile(configPath, optional: true, reloadOnChange: true);
}

void ConfigureService(IServiceCollection services, ConfigurationManager config)
{
    var redisConfigured = config["Cache:RedisConfigured"];
    int timeout = GetShutdownTimeout();

    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddControllers();
    services.AddHttpClient();
    services.AddSingleton<IHttpContextFactory>(sp => new TracingHttpContextFactory(sp));
    ConfigureOtel(services, config);
    if (redisConfigured == "True")
    {
        services.AddStackExchangeRedisCache(opttions =>
        {
            opttions.Configuration = config.GetSection("Redis")["ConnectionString"];
        });
        services.AddDownstreamService<RedisChacheDownstreamService>();
    }
    else if (config["Cache:MemCache"] == "True")
    {
        services.AddMemoryCache();
        services.AddDownstreamServiceWithCustomHandler<MemCacheDownStreamService, ForwardForHttpClientHandler>();
    }
    else
    {
        services.AddDownstreamService<SimpleDownstreamService>();
    }

    services.AddTransient<IRequestHandler, RequestHandler>();
    services.AddTransient<IEventReceiver, EventsReceiver1>();
    services.AddTransient<IEventReceiver, EventsReceiver2>();
    services.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromSeconds(timeout); });
}

void ConfigureOtel(IServiceCollection services, ConfigurationManager config)
{
    if (!"true".Equals(config["Otel:Enabled"]))
    {
        return;
    }
    var otel = services.AddOpenTelemetry();
    var otelpEndpoint = config["Otel:Endpoint"];
    var resurceBuilder = ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector();
    // Read and add k8s.pod.uuid if envvar set
    var podUid = Environment.GetEnvironmentVariable("POD_UID");
    if (!string.IsNullOrEmpty(podUid))
    {
        resurceBuilder.AddAttributes(new Dictionary<string, object>
        {
            ["k8s.pod.uid"] = podUid
        });
    }
    otel.WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.SetResourceBuilder(resurceBuilder);
    });
    otel.WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.SetResourceBuilder(resurceBuilder);
    });
    if (!string.IsNullOrEmpty(otelpEndpoint))
    {
        otel.UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otelpEndpoint));
    }
}

int GetShutdownTimeout()
{
    var envvar = Environment.GetEnvironmentVariable("SHUTDOWN_TIMEOUT");
    if (Int32.TryParse(envvar, out int timeout))
    {
        return timeout;
    }
    else
    {
        return 5;
    }
}