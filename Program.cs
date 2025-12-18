using hello_dotnet.Downstream;
using hello_dotnet.Factories;
using hello_dotnet.Invokers;
using hello_dotnet.Receivers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

if ("true".Equals(builder.Configuration["Otel:Enabled"]))
{
    builder.Logging.AddOpenTelemetry(logging =>
    {
        var endpoint = builder.Configuration["Otel:Endpoint"];
        if (!string.IsNullOrEmpty(endpoint))
        {
            logging.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(endpoint);
            });
        }
    });
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
    if (string.IsNullOrEmpty(otelpEndpoint))
    {
        return;
    }

    otel.WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();
        metrics.AddHttpClientInstrumentation();
        metrics.AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri(otelpEndpoint);
        });
    });
    otel.WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddOtlpExporter(otlp =>
        {
            otlp.Endpoint = new Uri(otelpEndpoint);
        });
    });
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