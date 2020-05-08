using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatumCollection.Core.Hosting
{
    /// <summary>
    /// 客户端主机
    /// </summary>
    public class SpiderHostBuilder : ISpiderHostBuilder
    {
        //服务集合
        private readonly ServiceCollection _services;

        private IServiceProvider _provider;

        //服务注入操作集合
        private ICollection<Action<IServiceCollection>> _configureServiceActions = 
            new List<Action<IServiceCollection>>();

        //应用配置操作集合
        private ICollection<Action<IConfigurationBuilder>> _configureAppConfigActions =
            new List<Action<IConfigurationBuilder>>();

        private IConfiguration _configuration;

        //主机是否创建
        private bool _hostBuilt = false;

        public SpiderHostBuilder()
        {
            _services = new ServiceCollection();
        }

        public ISpiderHostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            if (configure != null)
            {
                _configureServiceActions.Add(configure);
            }
            return this;
        }

        /// <summary>
        /// 注册日志服务
        /// Injected by ILoggingBuilder
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ISpiderHostBuilder ConfigureLogService(Action<ILoggingBuilder> configure)
        {
            if(Log.Logger == null)
            {
                var loggerConfig = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.RollingFile($"{AppDomain.CurrentDomain.BaseDirectory}/logs/system.log");
                Log.Logger = loggerConfig.CreateLogger();
            }
            
            _services.AddLogging(configure);
            return this;
        }

        /// <summary>
        /// 注册应用配置
        /// Json文件
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ISpiderHostBuilder ConfigureAppConfig(Action<IConfigurationBuilder> configure)
        {
            if (configure != null)
            {
                _configureAppConfigActions.Add(configure);
            }

            return this;
        }

        public ISpiderHost Build()
        {
            if (_hostBuilt)
            {
                throw new InvalidOperationException("services can build just once.");
            }
            _hostBuilt = true;

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            foreach (var configure in _configureAppConfigActions)
            {
                configure?.Invoke(configurationBuilder);
            }
            _configuration = configurationBuilder.Build();
            _services.AddSingleton(_configuration);

            foreach (var configure in _configureServiceActions)
            {
                configure?.Invoke(_services);
            }
            _provider = _services.BuildServiceProvider();

            //background service
            var backgroundServices = _provider.GetRequiredService<IEnumerable<IHostedService>>();
            foreach (IHostedService service in backgroundServices)
            {
                service.StartAsync(CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
            }

            return default(ISpiderHost);
        }
    }
}
