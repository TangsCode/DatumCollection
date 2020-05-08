using DatumCollection.Exceptions;
using DatumCollection.Spiders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatumCollection
{
    /// <summary>
    /// 主机托管服务
    /// </summary>
    public class HostBuilder
    {
        //服务集合
        private readonly ServiceCollection _services;

        private IServiceProvider _serviceProvider;

        //注册操作集合
        private List<Action<IServiceCollection>> _configureServicesActions =
            new List<Action<IServiceCollection>>();

        //注册配置操作集合
        private List<Action<IConfigurationBuilder>> _configureAppConfigActions = 
            new List<Action<IConfigurationBuilder>>();

        //程序配置
        private IConfiguration _configuration;

        //服务是否创建
        private bool _hostBuilt = false;

        public HostBuilder()
        {
            _services = new ServiceCollection();
        }

        public HostBuilder ConfigureServices(Action<IServiceCollection> configure)
        {
            if (configure != null)
            {
                _configureServicesActions.Add(configure);
            }

            return this;
        }

        /// <summary>
        /// 注册日志服务
        /// Injected by ILoggingBuilder
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HostBuilder ConfigureLogService(Action<ILoggingBuilder> configure)
        {
            _services.AddLogging(configure);
            return this;
        }

        /// <summary>
        /// 注册应用配置
        /// Json文件
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HostBuilder ConfigureConfigService(Action<IConfigurationBuilder> configure)
        {
            if (configure != null)
            {
                _configureAppConfigActions.Add(configure);
            }

            return this;
        }

        public HostBuilder Register<T>()
        {
            if (!typeof(AbstractSpider).IsAssignableFrom(typeof(T)))
            {
                throw new SpiderException($"{typeof(T)} is not an implement of AbstractSpider");
            }

            _services.AddTransient(typeof(T));
            return this;
        }

        public ServiceProvider Build()
        {
            if (_hostBuilt)
            {
                throw new InvalidOperationException("services can build just once.");
            }
            _hostBuilt = true;

            //build appconfig service
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            foreach (var configure in _configureAppConfigActions)
            {
                configure?.Invoke(configurationBuilder);
            }
            _configuration = configurationBuilder.Build();

            //add services to service collection
            _services.AddSingleton(_configuration);            
            _services.AddScoped<SystemOptions>();
            _services.AddScoped<SpiderParameters>();
            _services.AddOptions();
            _services.AddLogging();

            foreach (var configure in _configureServicesActions)
            {
                configure?.Invoke(_services);
            }
            _serviceProvider = _services.BuildServiceProvider();
            
            //background service
            var backgroundServices = _serviceProvider.GetRequiredService<IEnumerable<IHostedService>>();
            foreach (IHostedService service in backgroundServices)
            {
                service.StartAsync(CancellationToken.None).ConfigureAwait(true).GetAwaiter().GetResult();
            }


            return new ServiceProvider(_serviceProvider);
        }
        
    }
}
