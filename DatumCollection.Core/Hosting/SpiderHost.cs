using DatumCollection.Configuration;
using DatumCollection.Core.Builder;
using DatumCollection.Infrastructure.Spider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.Core.Hosting
{
    public class SpiderHost : ISpiderHost
    {
        private IStartup _startup;
        private bool _stopped;

        private PiplineListener _piplineListener;
        private ApplicationLifetime _applicationLifetime;
        private HostedServiceExecutor _hostedServiceExecutor;

        private readonly IServiceCollection _applicationServiceCollection;
        private readonly IServiceProvider _hostingServiceProvider;
        private readonly IConfiguration _config;
        private readonly AggregateException _hostingStartupErrors;

        //private readonly IApplicationBuilderFactory _applicationBuilderFactory;

        private IServiceProvider _applicationServices;
        private ExceptionDispatchInfo _applicationServicesException;
        private ILogger _logger = NullLogger.Instance;

        public IServiceProvider Services
        {
            get { return _applicationServices; }
        }

        public SpiderHost(
            IServiceCollection services,
            IServiceProvider hostingServiceProvider,
            IConfiguration config,
            AggregateException hostingStartupErrors
            )
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _hostingServiceProvider = hostingServiceProvider ?? throw new ArgumentNullException(nameof(hostingServiceProvider));
            _hostingStartupErrors = hostingStartupErrors;
            _applicationServiceCollection = services ?? throw new ArgumentNullException(nameof(services));

            _applicationServiceCollection.AddSingleton<PiplineListener>();
            _applicationServiceCollection.AddSingleton<ApplicationLifetime>();
            _applicationServiceCollection.AddSingleton<HostedServiceExecutor>();
            _applicationServiceCollection.AddSingleton<ILoggerFactory,LoggerFactory>();
            //_applicationServices = _applicationServiceCollection.BuildServiceProvider();

            //_logger = _applicationServices.GetRequiredService<ILoggerFactory>().CreateLogger<SpiderHost>();            
        }

        public void Initialize()
        {
            try
            {
                EnsureApplicationServices();
            }
            catch (Exception e)
            {
                if (_applicationServices == null)
                {
                    _applicationServices = _applicationServiceCollection.BuildServiceProvider();
                }

                _applicationServicesException = ExceptionDispatchInfo.Capture(e);
            }
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            _startup = _hostingServiceProvider.GetService<IStartup>();

            if (_startup == null)
            {
                throw new InvalidOperationException($"No application configured, please specify startup via ISpiderHostBuilder.Configure, injecting {nameof(IStartup)}");
            }
        }

        public void Dispose()
        {
            //to be developed,disposing hosting resources

        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger = _applicationServices.GetRequiredService<ILoggerFactory>().CreateLogger<SpiderHost>();
            _logger.Starting();

            //application pipline build
            var application = BuildApplication();

            _piplineListener = _applicationServices.GetRequiredService<PiplineListener>();
            _piplineListener.BeginListen(application);

            _applicationLifetime = _applicationServices.GetRequiredService<ApplicationLifetime>();
            _hostedServiceExecutor = _applicationServices.GetRequiredService<HostedServiceExecutor>();

            //fire IHostedService.Start()
            await Task.Factory.StartNew(async () =>
             {
                 await _hostedServiceExecutor.StartAsync(cancellationToken);
             });
                        
            _applicationLifetime?.NotifyStarted();
            _logger.Started();

            if (_hostingStartupErrors != null)
            {
                foreach (var exception in _hostingStartupErrors.InnerExceptions)
                {
                    _logger.HostingStartupAssemblyError(exception);
                }
            }
        }

        private PiplineDelegate BuildApplication()
        {
            try
            {
                _applicationServicesException?.Throw();

                var builderFactory = _applicationServices.GetRequiredService<IApplicationBuilderFactory>();
                var builder = builderFactory.CreateBuilder();
                builder.ApplicationServices = _applicationServices;

                Action<IApplicationBuilder> configure = _startup.Configure;

                configure(builder);
                return builder.Build();
            }
            catch (Exception e)
            {
                _logger.LogError("application build error :{0}", e);
                return context => {
                    return Task.CompletedTask;
                };
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_stopped)
            {
                return;
            }
            _stopped = true;

            _logger.Shutdown();
            
            var clientConfig = _applicationServices.GetRequiredService<SpiderClientConfiguration>();
            var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(clientConfig.SpiderHostTimeout)).Token;
            if (!cancellationToken.CanBeCanceled)
            {
                cancellationToken = timeoutToken;
            }
            else
            {
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken).Token;
            }

            //fire IApplicationLifetime.Stopping
            _applicationLifetime?.StopApplication();

            //fire the IHostedService.Stop
            if (_hostedServiceExecutor != null)
            {
                await _hostedServiceExecutor.StopAsync(cancellationToken).ConfigureAwait(false);
            }

            //fire pipline listenner
            _piplineListener?.EndListen();

            //release native memory
            var collector = _applicationServices.GetRequiredService<Infrastructure.Abstraction.ICollector>();
            collector.Dispose();

            _applicationLifetime?.NotifyStopped();

        }
    }
}
