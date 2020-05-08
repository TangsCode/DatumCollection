using DatumCollection.Common;
using DatumCollection.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Spiders
{

    public abstract class AbstractSpider 
    {

        protected IEventBus _mq { get; }

        /// <summary>
        /// 日志接口
        /// </summary>
        protected ILogger _logger { get;}

        /// <summary>
        /// 系统选项
        /// </summary>
        public SystemOptions _options { get;}

        /// <summary>
        /// Quartz scheduler
        /// </summary>
        protected IScheduler _scheduler { get; }

        /// <summary>
        /// 爬虫服务
        /// </summary>
        protected IServiceProvider _services { get; }

        public AbstractSpider(SpiderParameters parameters)
        {
            _mq = parameters.EventBus;
            _logger = parameters.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            _options = parameters.SystemOptions;
            //_scheduler = parameters.Scheduler;
            _services = parameters.ServiceProvider;
        }

        /// <summary>
        /// Run a spider asynchronously
        /// </summary>
        /// <returns></returns>
        public abstract Task RunAsync();

    }
}
