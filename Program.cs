using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
