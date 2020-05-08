using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatumCollection.Collectors;
using DatumCollection.Common;
using DatumCollection.Data;
using DatumCollection.EventBus;
using DatumCollection.HtmlParser;
using DatumCollection.SpiderScheduler;
using DatumCollection.WebDriver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DatumCollection.SpiderStrategies
{
    /// <summary>
    /// 爬虫策略抽象
    /// </summary>
    public abstract class AbstractSpiderStrategy : BackgroundService
    {
        /// <summary>
        /// 消息队列
        /// </summary>
        protected readonly IEventBus _mq;

        /// <summary>
        /// 系统选项(配置化)
        /// </summary>
        protected readonly SystemOptions _options;

        protected readonly IDataStorage _storage;

        protected ISpiderScheduler _scheduler;

        protected ICollector _collector;

        /// <summary>
        /// 日志接口
        /// </summary>
        protected readonly ILogger _logger;

        protected AbstractSpiderStrategy(
            IEventBus eventBus,
            SystemOptions systemOptions,
            ILogger<AbstractSpiderStrategy> logger,
            IDataStorage storage,
            ISpiderScheduler scheduler,
            ICollector collector)
        {
            _mq = eventBus;
            _options = systemOptions;
            _logger = logger;
            _scheduler = scheduler;
            _storage = storage;
            _collector = collector;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //subscribe source message
            _mq.Subscribe(_options.TopicScheduleObserver, async message =>
             {
                 if (message == null)
                 {
                     _logger.LogWarning("Receive empty message");
                     return;
                 }
                 if (message.IsTimeout(60))
                 {
                     _logger.LogWarning($"Message is timeout: {JsonConvert.SerializeObject(message)}");
                 }
                 if (Enum.TryParse(typeof(MessageType), message.Type, out var messageType))
                 {
                     switch (messageType)
                     {
                         case MessageType.GoodsSpider:
                         case MessageType.ShopSpider:
                             {
                                 var sources = JsonConvert.DeserializeObject<List<dynamic>>(message.Data);
                                 _logger.LogInformation($"receive spider message");

                                 //schedule
                                 dynamic schedule = new ExpandoObject();
                                 schedule.CollectType = (MessageType)messageType == MessageType.GoodsSpider
                                 ? CollectType.Goods : CollectType.Shop;
                                 schedule.ID = Guid.NewGuid().ToString();
                                 schedule.FK_ScheduleConfig_ID = sources[0].FK_ScheduleConfig_ID.Value;
                                 schedule.StartUpTime = DateTime.Now;
                                 schedule.SuccessCount = 0;
                                 schedule.FailedCount = 0;
                                 if ((MessageType)messageType == MessageType.GoodsSpider)
                                 {
                                     schedule.TotalCount = sources.Count;
                                 }

                                 await _storage.ExecuteAsync(new SqlContext("ScheduleRecord", properties: schedule, operation: Operation.Insert));

                                 _logger.LogInformation($"schedule[{schedule.ID}] begins, count {sources.Count}");
                                 try
                                 {
                                     CollectContext context = new CollectContext(sources, schedule);
                                     context.CollectType = ((MessageType)messageType == MessageType.GoodsSpider) ? CollectType.Goods : CollectType.Shop;
                                     _scheduler.HandleScheduleTask(context).GetAwaiter().GetResult();
                                     _logger.LogInformation($"schedule[{schedule.ID}]任务完成");
                                 }
                                 catch (Exception e)
                                 {
                                     _logger.LogError($"schedule[{schedule.ID}] 异常退出.\r\n{e.ToString()}");
                                     break;
                                 }

                                 break;
                             }
                         case MessageType.DisposeResources:
                             {
                                 //_logger.LogInformation("idle wait too long,now disposing resources.");
                                 //foreach (var driver in drivers)
                                 //{
                                 //    driver.Dispose();
                                 //}
                                 break;
                             }
                         default:
                             {
                                 break;
                             }
                     }
                 }
                 else
                 {
                     _logger.LogWarning($"Receive undefined-type message:{message.Type}");
                 }
             });

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            //webDriver?.Dispose();
        }
    }
}
