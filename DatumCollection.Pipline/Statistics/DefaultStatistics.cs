using DatumCollection.Configuration;
using DatumCollection.Data;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Pipline.Statistics
{
    /// <summary>
    /// 默认统计器
    /// </summary>
    public class DefaultStatistics : IStatistics
    {
        private readonly ILogger<DefaultStatistics> _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly IDataStorage _storage;

        public DefaultStatistics(
            ILogger<DefaultStatistics> logger,
            IMessageQueue mq,
            SpiderClientConfiguration config,
            IDataStorage storage)
        {
            _logger = logger;
            _mq = mq;
            _config = config;
            _storage = storage;
        }

        public async Task Analyze(SpiderContext context)
        {
            try
            {
                context.Task.FinishTime = DateTime.Now;
                context.Task.SuccessCount = context.SpiderAtoms.Count(a => a.SpiderStatus == SpiderStatus.OK);
                context.Task.FailedCount = context.SpiderAtoms.Count(a => a.SpiderStatus != SpiderStatus.OK);
                await _storage.Insert(new[] { context.Task });
                _logger.LogInformation("Task[{task}] statistics:\r\n success:{success} fail:{fail} toal:{total} completed in {elap} secs.",
                    context.Task.Id, context.Task.SuccessCount, context.Task.FailedCount, context.SpiderAtoms.Count, context.Task.ElapsedTime);
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());                
            }
            
        }
    }
}
