using DatumCollection.Core.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DatumCollection.Core
{
    /// <summary>
    /// dotnet core startup
    /// </summary>
    public class Startup
    {

        public static void Execute()
        {
            try
            {
                var builder = new SpiderHostBuilder();
                builder.ConfigureLogService(c => c.SetMinimumLevel(LogLevel.Information).AddSerilog());
                builder.ConfigureAppConfig(b =>
                b.AddJsonFile("appsettings.json")
                );
                builder.ConfigureServices(services =>
                {
                    services.AddMessageQueue(b => b.UseKafka());
                    //services.AddDataStorage(
                    //    (c) => c.UseSqlServer());
                    ////services.AddQuartz();
                    //services.AddSpiderStrategy(
                    //    (c) => c.UseWebDriver());
                    //services.AddScheduleObserver();
                    //services.AddSpiderScheduler();
                    //services.AddSpiderCollector();
                    //services.AddImageRecognizer();
                    //services.AddCache();
                });
                builder.Build().Run();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
