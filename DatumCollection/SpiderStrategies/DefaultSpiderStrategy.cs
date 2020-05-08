using DatumCollection.Collectors;
using DatumCollection.Data;
using DatumCollection.EventBus;
using DatumCollection.SpiderScheduler;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.SpiderStrategies
{
    /// <summary>
    /// 默认爬虫策略
    /// </summary>
    public class DefaultSpiderStrategy : AbstractSpiderStrategy
    {
        public DefaultSpiderStrategy(
            IEventBus eventBus,
            SystemOptions systemOptions,
            ILogger<DefaultSpiderStrategy> logger,
            IDataStorage storage,
            ISpiderScheduler scheduler,
            ICollector collector) 
            : base(eventBus, systemOptions, logger, storage, scheduler, collector)
        {

        }
    }
}
