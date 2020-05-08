using DatumCollection.Core.Builder;
using DatumCollection.MessageQueue;
using DatumCollection.MessageQueue.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core
{
    public static class ServiceCollectionExtension
    {
        #region Message Queue (supporting Kafka & EventBus)
        public static IServiceCollection AddMessageQueue(this IServiceCollection services, 
            Action<MessageQueueBuilder> action = null)
        {
            services.AddSingleton<IMessageQueue, KafkaMessageQueue>();
            var builder = new MessageQueueBuilder(services);
            action?.Invoke(builder);

            return services;
        }

        public static MessageQueueBuilder UseKafka(this MessageQueueBuilder builder)
        {
            builder.Services.AddSingleton<IMessageQueue, KafkaMessageQueue>();
            return builder;
        }
        #endregion

        #region Data Storage (supporting Sql Server)
        public static IServiceCollection AddDataStorage(this IServiceCollection services)
        {
            services.AddSingleton<IDataStorage, >
            return services;
        }
        #endregion
    }
}
