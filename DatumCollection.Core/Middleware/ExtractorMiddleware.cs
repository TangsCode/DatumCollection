using DatumCollection.Configuration;
using DatumCollection.Data.Entities;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
using DatumCollection.Utility.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Middleware
{
    /// <summary>
    /// extractor middleware
    /// which extracts data from content collected already
    /// </summary>
    public class ExtractorMiddleware
    {
        private readonly PiplineDelegate _next;
        private readonly ILogger _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly IExtractor _extractor;

        public ExtractorMiddleware(
            PiplineDelegate next,
            IMessageQueue mq,
            IExtractor extractor,
            SpiderClientConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _config = config;
            _extractor = extractor;
            _logger = loggerFactory.CreateLogger<ExtractorMiddleware>();
            _mq = mq;
        }

        public async Task InvokeAsync(SpiderContext context)
        {
            try
            {
                _logger.LogInformation("Task[{task}] reaches {middleware}", context.Task.Id, nameof(ExtractorMiddleware));
                Parallel.ForEach(context.SpiderAtoms, async atom =>
                {
                    atom.Model = await _extractor.ExtractAsync(atom);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error occured in {middleware}", nameof(ExtractorMiddleware));
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
