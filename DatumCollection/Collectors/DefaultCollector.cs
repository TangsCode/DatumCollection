using DatumCollection.Data;
using DatumCollection.EventBus;
using DatumCollection.Utility.Helper;
using DatumCollection.Utility.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

namespace DatumCollection.Collectors
{
    /// <summary>
    /// 默认采集器
    /// </summary>
    public class DefaultCollector : ICollector
    {
        protected readonly ILogger<DefaultCollector> _logger;
        protected readonly IEventBus _mq;
        protected readonly IDataStorage _storage;
        protected readonly SystemOptions _options;

        public DefaultCollector(
            ILogger<DefaultCollector> logger,
            IEventBus mq,
            IDataStorage storage,
            SystemOptions options)
        {
            _logger = logger;
            _mq = mq;
            _storage = storage;
            _options = options;
        }

        protected string DownloadPath =>
            _options.ImageDownloadPath.NotNull()
            ? Path.Join(_options.ImageDownloadPath, DateTimeHelper.TodayString)
            : Path.Join(SpiderEnvironment.BaseDirectory, "Images", DateTimeHelper.TodayString);

        public CollectResult Collect(CollectContext context)
        {
            CollectResult result = new CollectResult(context.Sources.Count());

            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
            }

            Parallel.ForEach(context.Sources,
                (source) =>
                {
                    WebRequest request = HttpWebRequest.Create(source.Url);

                    using (WebClient client = new WebClient())
                    {
                        
                    }
                });

            result.Success = true;
            return result;
        }

        public Task<CollectResult> CollectAsync(CollectContext context)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
