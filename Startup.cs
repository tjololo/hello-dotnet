using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using hello_dotnet.Downstream;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace hello_dotnet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var redisConfigured = Configuration["Cache:RedisConfigured"];
            int timeout = GetShutdownTimeout();
            services.AddControllers();
            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();
            if (redisConfigured == "True")
            {
                services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>((options) =>
                {
                    return Configuration.GetSection("Redis").Get<RedisConfiguration>();
                });
                services.AddScoped<IDownstreamService, RedisChacheDownstreamService>();
            }
            else if (Configuration["Cache:MemCache"] == "True")
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private int GetShutdownTimeout()
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
    }
}
