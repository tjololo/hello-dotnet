using hello_dotnet.Downstream;
using hello_dotnet.Factories;
using hello_dotnet.Invokers;
using hello_dotnet.Receivers;

var builder = WebApplication.CreateBuilder(args);
ConfigureApp(builder.Configuration);
ConfigureService(builder.Services, builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

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
    services.AddControllers();
    services.AddHttpClient();
    services.AddApplicationInsightsTelemetry();
    services.AddSingleton<IHttpContextFactory>(sp => new TracingHttpContextFactory(sp));
    if (redisConfigured == "True")
    {
        services.AddStackExchangeRedisCache(opttions =>
        {
            opttions.Configuration = config.GetSection("Redis")["ConnectionString"];
        });
        services.AddScoped<IDownstreamService, RedisChacheDownstreamService>();
    }
    else if (config["Cache:MemCache"] == "True")
    {
        services.AddMemoryCache();
        services.AddScoped<IDownstreamService, MemCacheDownStreamService>();
    }
    else
    {
        services.AddScoped<IDownstreamService, SimpleDownstreamService>();
    }

    services.AddTransient<IRequestHandler, RequestHandler>();
    services.AddTransient<IEventReceiver, EventsReceiver1>();
    services.AddTransient<IEventReceiver, EventsReceiver2>();
    services.Configure<HostOptions>(options => { options.ShutdownTimeout = TimeSpan.FromSeconds(timeout); });
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