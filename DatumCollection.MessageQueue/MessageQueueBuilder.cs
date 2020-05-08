using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.MessageQueue
{
    public class MessageQueueBuilder
    {
        public IServiceCollection Services { get; }

        public MessageQueueBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
