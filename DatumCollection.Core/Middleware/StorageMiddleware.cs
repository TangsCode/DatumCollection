using DatumCollection.Configuration;
using DatumCollection.Data;
using DatumCollection.Data.Entities;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
using DatumCollection.Utility.Extensions;
using DatumCollection.Utility.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Middleware
{
    /// <summary>
    /// storage middleware
    /// is aimed to storage data that is extracted    
    /// </summary>
    public class StorageMiddleware
    {
        private readonly PiplineDelegate _next;
        private readonly ILogger _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly IStorage _storage;

        public StorageMiddleware(
            PiplineDelegate next,
            IMessageQueue mq,
            IStorage storage,
            SpiderClientConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<StorageMiddleware>();
            _mq = mq;
            _config = config;
            _storage = storage;
        }

        public async Task InvokeAsync(SpiderContext context)
        {
            try
            {
                _logger.LogInformation("Task[{task}] reaches {middleware}", context.Task.Id, nameof(StorageMiddleware));
                Parallel.ForEach(context.SpiderAtoms.StatusOk(), async atom =>
                 {
                     if (atom != null)
                     {
                         await _storage.Store(atom);
                     }
                 });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error occured in {middleware}", nameof(StorageMiddleware));
                await _mq.PublishAsync(_config.TopicStatisticsFail, new Message
                {
                    Data = $"task {context.Task.Id} error occurred in {e.StackTrace} => {e.Message}",
                    PublishTime = (long)DateTimeHelper.GetCurrentUnixTimeNumber()
                });
            }
            await _next(context);
        }
    }
}
