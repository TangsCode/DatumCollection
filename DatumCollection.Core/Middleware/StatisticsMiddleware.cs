using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatumCollection.Configuration;
using DatumCollection.Data;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;

namespace DatumCollection.Core.Middleware
{
    /// <summary>
    /// 统计分析中间件
    /// </summary>
    public class StatisticsMiddleware
    {
        private readonly PiplineDelegate _next;
        private readonly ILogger _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly IStatistics _statistics;
        private readonly IDataStorage _storage;

        public StatisticsMiddleware(
            PiplineDelegate next,
            IMessageQueue mq,
            SpiderClientConfiguration config,
            ILoggerFactory loggerFactory,
            IStatistics statistics,
            IDataStorage storage)
        {
            _next = next;
            _mq = mq;
            _config = config;
            _logger = loggerFactory.CreateLogger<StatisticsMiddleware>();
            _statistics = statistics;
            _storage = storage;
        }

        public async Task InvokeAsync(SpiderContext context)
        {
            try
            {
                _logger.LogInformation("Task[{task}] reaches {middleware}", context.Task.Id, nameof(StatisticsMiddleware));
                await _statistics.Analyze(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error occured in {middleware}", nameof(StatisticsMiddleware));
            }

            await _next(context);
        }
    }
}
