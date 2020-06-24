using DatumCollection.Configuration;
using DatumCollection.Core.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
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

        private SpiderClientConfiguration _config;

        //主机是否创建
        private bool _hostBuilt = false;

        public SpiderHostBuilder()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(SpiderEnvironment.ApplicationRootPath)                
                .AddJsonFile(SpiderEnvironment.DefaultSettings, true, reloadOnChange:true)
                .Build();
            _config = new SpiderClientConfiguration(_configuration);
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
            var loggerConfig = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.RollingFile($"{AppDomain.CurrentDomain.BaseDirectory}/logs/system.log");
            Log.Logger = loggerConfig.CreateLogger();

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

            var hostingServices = BuildCommonServices(out var hostingStartupErrors);
            var applicationServices = hostingServices.Clone();
            var hostingServiceProvider = GetProviderFromFactory(hostingServices);

            var host = new SpiderHost(
                applicationServices,
                hostingServiceProvider,
                _configuration,
                hostingStartupErrors);
            try
            {
                host.Initialize();
                _ = host.Services.GetService<IConfiguration>();

                return host;
            }
            catch (Exception)
            {
                host.Dispose();
                throw;
            }
                        
        }

        IServiceProvider GetProviderFromFactory(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

            if (factory != null)
            {
                using (provider)
                {
                    return factory.CreateServiceProvider(factory.CreateBuilder(services));
                }
            }

            return provider;
        }

        private IServiceCollection BuildCommonServices(out AggregateException hostingStartupErrors)
        {
            hostingStartupErrors = null;

            var _services = new ServiceCollection();
            _services.AddSingleton(_ => _configuration);
            _services.AddSingleton<SpiderClientConfiguration>();
            _services.AddTransient<IApplicationBuilderFactory, ApplicationBuilderFactory>();
            _services.AddOptions();
            _services.AddLogging();
                         
            //startup assembly configured in json file
            if (!string.IsNullOrEmpty(_config.SpiderHostStartupAssembly))
            {
                try
                {
                    var startupType = FindStartupType(_config.SpiderHostStartupAssembly, _config.SpiderHostStartupType);
                    if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType))
                    {
                        _services.AddSingleton(typeof(IStartup), startupType);
                    }
                }
                catch (Exception e)
                {
                    var capture = ExceptionDispatchInfo.Capture(e);
                    _services.AddSingleton<IStartup>(_ =>
                    {
                        capture.Throw();
                        return null;
                    });
                }

            }

            //startup assembly configured in program
            foreach (var configure in _configureServiceActions)
            {
                configure(_services);
            }

            return _services;
        }

        public static Type FindStartupType(string startupAssemblyName, string startupType)
        {
            if (string.IsNullOrEmpty(startupAssemblyName))
            {
                throw new ArgumentException(
                    string.Format("startup assembly is required if specifying, {0} can not be null or empty.",
                    nameof(startupAssemblyName)));
            }

            var assembly = Assembly.Load(new AssemblyName(startupAssemblyName));
            if (assembly == null)
            {
                throw new InvalidOperationException(string.Format("the assembly {0} failed to laod", startupAssemblyName));
            }

            if (string.IsNullOrEmpty(startupType))
            {
                startupType = "Startup";
            }

            var type = assembly.GetType(startupType);
            if (type == null)
            {
                throw new InvalidOperationException(string.Format("a type named [{0}] could not be found in assembly {1}",
                    startupType, startupAssemblyName));
            }

            return type;
        }
    }
}
