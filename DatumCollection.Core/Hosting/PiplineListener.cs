
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatumCollection.Infrastructure.Spider;
using DatumCollection.MessageQueue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatumCollection.Core.Hosting
{
    public class PiplineListener
    {
        private readonly ILogger<PiplineListener> _logger;
        private IMessageQueue _mq;

        public PiplineListener(
            ILogger<PiplineListener> logger,
            IMessageQueue mq)
        {
            _logger = logger;
            _mq = mq;
        }

        public void BeginListen(PiplineDelegate app)
        {
            _mq.Subscribe(MessageType.SpiderRequest.ToString(), async message =>
            {
                var context = message.Data as SpiderContext;
                await app.Invoke(context);
            });
        }

        public void EndListen()
        {
            _mq.Unsubscribe(MessageType.SpiderRequest.ToString());
            _mq?.Dispose();
        }
    }
}
