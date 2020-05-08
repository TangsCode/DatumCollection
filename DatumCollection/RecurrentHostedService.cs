using DatumCollection.Data;
using DatumCollection.EventBus;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection
{

    /// <summary>
    /// 托管周期计划
    /// </summary>
    public abstract class RecurrentHostedService : IHostedService, IDisposable
    {
        protected readonly ILogger<RecurrentHostedService> _logger;
        protected readonly IEventBus _mq;
        protected readonly SystemOptions _options;
        protected readonly IDataStorage _storage;
        protected Timer _timer;
        protected DateTime lastRunningTime = DateTime.Now;

        public abstract int RecurrentSeconds { get; protected set; }

        public RecurrentHostedService(
            ILogger<RecurrentHostedService> logger,
            IEventBus mq,
            SystemOptions options,
            IDataStorage storage)
        {
            _logger = logger;
            _mq = mq;
            _options = options;
            _storage = storage;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{this.ToString()} now begins with pulse of every {RecurrentSeconds} seconds");
            _timer = new System.Threading.Timer(RecurrentWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(RecurrentSeconds));
            return Task.CompletedTask;
        }

        public virtual void RecurrentWork(object state)
        {
            _logger.LogInformation($"{this.ToString()} is heartbeating");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{this.ToString()} is quiting");
            _timer.Dispose();

            return Task.CompletedTask;
        }
    }
}
