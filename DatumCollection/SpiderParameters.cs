using DatumCollection.Common;
using DatumCollection.Data;
using DatumCollection.EventBus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection
{
    public class SpiderParameters
    {
        internal IEventBus EventBus { get; }        

        internal SystemOptions SystemOptions { get; }

        internal IDataStorage Storage { get; }

        //internal IScheduler Scheduler { get; }

        internal IServiceProvider ServiceProvider { get; }

        public SpiderParameters(IEventBus eventBus,
            SystemOptions systemOptions,
            IServiceProvider serviceProvider
            //IScheduler scheduler
            )
        {
            EventBus = eventBus;
            SystemOptions = systemOptions;
            //Scheduler = scheduler;
            ServiceProvider = serviceProvider;
        }
    }
}
