using DatumCollection.Utility.Extensions;
using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Threading;
using DatumCollection.Spiders;
using DatumCollection.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using DatumCollection.Data;
using System.Dynamic;
using DatumCollection.SpiderStrategies;
using Hangfire;
using System.Net.Http;

namespace DatumCollection
{
    public abstract class StartUp
    {
        /// <summary>
        /// Create logger for current application.
        /// </summary>
        public static void CreateLogger()
        {
            var configure = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.RollingFile($"{SpiderEnvironment.BaseDirectory}/logs/system.log");
            Log.Logger = configure.CreateLogger();
        }

        /// <summary>
        /// 执行爬虫
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ExecuteSpider<T>()
        {
            try
            {
                CreateLogger();
                var builder = new HostBuilder();
                builder.ConfigureLogService(b => b.SetMinimumLevel(LogLevel.Information).AddSerilog());
                builder.ConfigureConfigService(b =>
                b.AddJsonFile("appsettings.json")
                .AddCommandLine(Environment.GetCommandLineArgs(), SpiderEnvironment.SwitchMappings)
                .AddEnvironmentVariables()
                );
                builder.ConfigureServices(services =>
                {
                    services.AddLocalMessagQueue();
                    services.AddDataStorage(
                        (c) => c.UseSqlServer());
                    //services.AddQuartz();
                    services.AddSpiderStrategy(
                        (c) => c.UseWebDriver());
                    services.AddScheduleObserver();
                    services.AddSpiderScheduler();
                    services.AddSpiderCollector();
                    services.AddImageRecognizer();
                    services.AddCache();
                });
                builder.Register<T>();
                var provider = builder.Build();
                var instance = provider.CreateSpider<T>();
                instance.RunAsync();
            }
            catch (Exception e)
            {
                Log.Logger.Error($"the program startup failed.error:{e}");
            }

        }

    }
}
