using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DatumCollection.Infrastructure.Web;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using System.Linq;
using DatumCollection.Utility.Helper;

namespace DatumCollection.Core.Middleware
{
    /// <summary>
    /// collector middleware 
    /// which supports downloading html file or dynamic content from web
    /// </summary>
    public class CollectorMiddlware
    {
        private readonly PiplineDelegate _next;
        private readonly ILogger _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly ICollector _collector;

        public CollectorMiddlware(
            PiplineDelegate next,
            IMessageQueue mq,
            ICollector collector,
            SpiderClientConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _config = config;
            _collector = collector;
            _logger = loggerFactory.CreateLogger<CollectorMiddlware>();
            _mq = mq;
        }

        public async Task InvokeAsync(SpiderContext context)
        {
            try
            {
                Parallel.ForEach(context.SpiderAtoms, async atom =>
                {
                    atom.Response = await _collector.CollectAsync(atom.Request);                    
                });
                _logger.LogInformation("{middleware} process {count} atoms completed.", nameof(CollectorMiddlware), context.SpiderAtoms.Count);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error occured in {middleware}", nameof(CollectorMiddlware));
                await _mq.PublishAsync(_config.TopicStatisticsFail, new Message {
                    Data = $"task {context.Task.Id} error occurred in {e.StackTrace} => {e.Message}",
                    PublishTime = (long)DateTimeHelper.GetCurrentUnixTimeNumber()
                });
            }

            await _next(context);
        }
    }      
}
