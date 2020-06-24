using DatumCollection.Core.Builder;
using DatumCollection.Data;
using DatumCollection.Data.SqlServer;
using DatumCollection.Data.MySql;
using DatumCollection.MessageQueue;
using DatumCollection.MessageQueue.Kafka;
using DatumCollection.MessageQueue.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using DatumCollection.HostedServices;
using DatumCollection.Pipline;
using DatumCollection.Infrastructure.Abstraction;
using DatumCollection.Pipline.Collector;
using DatumCollection.Pipline.Collectors;
using DatumCollection.HostedServices.Schedule;

namespace DatumCollection.Core
{
    public static class ServiceCollectionExtension
    {
        #region Message Queue (supporting Kafka,RabbitMQ & EventBus)
        public static IServiceCollection AddMessageQueue(this IServiceCollection services, 
            Action<MessageQueueServiceBuilder> action = null)
        {
            services.AddSingleton<IMessageQueue, KafkaMessageQueue>();
            var builder = new MessageQueueServiceBuilder(services);
            action?.Invoke(builder);

            return services;
        }

        public static MessageQueueServiceBuilder UseRabbitMQ(this MessageQueueServiceBuilder builder)
        {
            builder.Services.AddSingleton<IMessageQueue, RabbitMessageQueue>();
            return builder;
        }

        public static MessageQueueServiceBuilder UseKafka(this MessageQueueServiceBuilder builder)
        {
            builder.Services.AddSingleton<IMessageQueue, KafkaMessageQueue>();
            return builder;
        }
        #endregion

        #region Data Storage (supporting Sql Server, MySql,PostgreSql & SQLite)
        public static IServiceCollection AddDataStorage(this IServiceCollection services,
            Action<DataStorageServiceBuilder> action = null
            )
        {
            services.AddSingleton<IDataStorage, SqlServerStorage>();
            var builder = new DataStorageServiceBuilder(services);
            action?.Invoke(builder);

            return services;
        }

        public static DataStorageServiceBuilder UseSqlServer(this DataStorageServiceBuilder builder)
        {
            builder.Services.AddSingleton<IDataStorage, SqlServerStorage>();
            return builder;
        }

        public static DataStorageServiceBuilder UseMySql(this DataStorageServiceBuilder builder)
        {
            builder.Services.AddSingleton<IDataStorage, MySqlStorage>();
            return builder;
        }
        #endregion

        #region Hosted Service
        public static IServiceCollection AddSpiderHostedService(this IServiceCollection services)
        {
            services.AddHostedService<SpiderScheduleHostedService>();
            return services;
        }
        #endregion

        #region Spider Collector
        public static IServiceCollection AddSpiderCollector(this IServiceCollection services,
            Action<PiplineServiceBuilder> action = null)
        {
            services.AddSingleton<ICollector, DefaultCollector>();
            var builder = new PiplineServiceBuilder(services);
            action?.Invoke(builder);

            return services;
        }

        public static PiplineServiceBuilder UseWebDriver(this PiplineServiceBuilder builder)
        {
            builder.Services.AddSingleton<ICollector, WebDriverCollector>();
            return builder;
        }
        #endregion

        public static IServiceCollection Clone(this IServiceCollection services)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in services)
            {
                clone.Add(service);
            }
            return clone;
        }
    }
}
