﻿using DatumCollection.Configuration;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
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
    public class StorageMiddleware : IMiddleware
    {
        private readonly PiplineDelegate _next;
        private readonly ILogger _logger;
        private readonly IMessageQueue _mq;
        private readonly SpiderClientConfiguration _config;
        private readonly ICollector _collector;

        public StorageMiddleware(
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

        public async Task InvokeAsync(SpiderContext context, PiplineDelegate next)
        {
            //do something
            await _next(context);
        }
    }
}
