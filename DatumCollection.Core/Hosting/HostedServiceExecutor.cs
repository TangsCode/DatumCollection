using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DatumCollection.Core.Hosting
{
    internal class HostedServiceExecutor
    {
        private readonly IEnumerable<IHostedService> _services;
        private readonly ILogger<HostedServiceExecutor> _logger;

        public HostedServiceExecutor(
            ILogger<HostedServiceExecutor> logger,
            IEnumerable<IHostedService> services)
        {
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken token)
        {
            return ExecuteAsync(service => service.StartAsync(token));
        }

        public Task StopAsync(CancellationToken token)
        {
            return ExecuteAsync(service => service.StopAsync(token));
        }

        private async Task ExecuteAsync(Func<IHostedService,Task> callback,bool throwOnFirstFailure = true)
        {
            List<Exception> exceptions = null;

            foreach (var service in _services)
            {
                try
                {
                    await callback(service);
                }
                catch (Exception e)
                {
                    if (throwOnFirstFailure) { throw; }

                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(e);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}