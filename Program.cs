using System;
using hello_dotnet.Downstream;
using hello_dotnet.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

void ConfigureApp(ConfigurationManager config)
{
    config.AddJsonFile("/config/appsettings.json", optional: true, reloadOnChange: true);
    string configBasePathEnv = Environment.GetEnvironmentVariable("CONFIG_PATH");
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
        services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>((options) =>
        {
            return config.GetSection("Redis").Get<RedisConfiguration>();
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
    services.Configure<HostOptions>(options =>
    {
        options.ShutdownTimeout = TimeSpan.FromSeconds(timeout);
    });
}

int GetShutdownTimeout()
{
    var envvar = System.Environment.GetEnvironmentVariable("SHUTDOWN_TIMEOUT");
    if (Int32.TryParse(envvar, out int timeout))
    {
        return timeout;
    }
    else
    {
        return 5;
    }
}

/*
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace hello_dotnet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingCtx, config) =>
                {
                    config.AddJsonFile("/config/appsettings.json", optional: true, reloadOnChange: true);
                    string configBasePathEnv = Environment.GetEnvironmentVariable("CONFIG_PATH");
                    string configPath = $"{configBasePathEnv}/appsettings.json";
                    config.AddJsonFile(configPath, optional: true, reloadOnChange: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
*/
