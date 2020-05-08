using DatumCollection.Data;
using DatumCollection.EventBus;
using DatumCollection.ImageRecognition;
using DatumCollection.Utility.Helper;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace DatumCollection.Collectors
{
    public abstract class AbstractCollector : ICollector, IHostedService
    {

        protected readonly ILogger<AbstractCollector> _logger;
        protected readonly IEventBus _mq;
        protected readonly IDataStorage _storage;
        protected readonly SystemOptions _options;
        protected readonly IRecognizer _recognizer;

        protected string DownloadPath =>
            _options.ImageDownloadPath.NotNull()
            ? Path.Join(_options.ImageDownloadPath, DateTimeHelper.TodayString)
            : Path.Join(SpiderEnvironment.BaseDirectory, "Images", DateTimeHelper.TodayString);

        public AbstractCollector(
            ILogger<AbstractCollector> logger,
            IEventBus mq,
            IDataStorage storage,
            SystemOptions options,
            IRecognizer recognizer)
        {
            _logger = logger;
            _mq = mq;
            _storage = storage;
            _options = options;
            _recognizer = recognizer;
        }

        public CollectResult Collect(CollectContext context)
        {
            return null;
        }

        public Task<CollectResult> CollectAsync(CollectContext context)
        {
            return null;
        }

        public virtual Task Init(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Init(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
