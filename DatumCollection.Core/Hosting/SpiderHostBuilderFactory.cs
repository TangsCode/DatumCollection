using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Hosting
{
    public class SpiderHostBuilderFactory
    {
        public ISpiderHostBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new SpiderHostBuilder();
            builder.ConfigureLogService(logging => {
                logging.ClearProviders();
                logging.AddConsole();
                //logging.AddDebug();
                //logging.AddEventSourceLogger();
                logging.SetMinimumLevel(LogLevel.Information).AddSerilog();
            });
            builder.ConfigureAppConfig(config =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });
            
            return builder;
        }
    }
}
