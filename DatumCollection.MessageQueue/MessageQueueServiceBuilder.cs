using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.MessageQueue
{
    public class MessageQueueServiceBuilder
    {
        public IServiceCollection Services { get; }

        public MessageQueueServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
