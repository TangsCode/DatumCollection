using DatumCollection.Common;
using DatumCollection.Data;
using DatumCollection.SpiderStrategies;
using DatumCollection.EventBus;
using DatumCollection.Utility;
using DatumCollection.WebDriver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using DatumCollection.Collectors;
using DatumCollection.ImageRecognition;
using DatumCollection.ServiceBuilder;
using Quartz.Impl;
using DatumCollection.SpiderScheduler;
using DatumCollection.Quartz;
using DatumCollection.Cache;

namespace DatumCollection
{
    /// <summary>
    /// 服务集合
    /// </summary>
    public static class ServiceCollectionExtension
    {

        #region Spider Strategy        
        public static IServiceCollection AddSpiderStrategy(this IServiceCollection services,
            Action<SpiderStrategyBuilder> configure = null)
        { 
            services.AddSingleton<IHostedService, DefaultSpiderStrategy>();
            var downloadStrategyBuilder = new SpiderStrategyBuilder(services);
            configure?.Invoke(downloadStrategyBuilder);

            return services;
        }

        public static SpiderStrategyBuilder UseWebDriver(this SpiderStrategyBuilder builder)
        {
            builder.Services.AddTransient<WebDriverBase>();

            return builder;
        }
        #endregion

        #region Collector
        public static IServiceCollection AddSpiderCollector(this IServiceCollection services,
            Action<SpiderCollectorBuilder> configure = null)
        {
            services.AddScoped<ICollector, WebDriverCollector>();
            var builder = new SpiderCollectorBuilder(services);
            configure?.Invoke(builder);

            return services;
        }
        #endregion

        #region Recognizer
        public static IServiceCollection AddImageRecognizer(this IServiceCollection services,
            Action<RecognizerBuilder> configure = null)
        {
            services.AddScoped<IRecognizer, AliCloudOCR>();
            var builder = new RecognizerBuilder(services);
            configure?.Invoke(builder);

            return services;
        }
        #endregion

        #region Data Storage

        public static IServiceCollection AddDataStorage(this IServiceCollection services,
            Action<DataStorageBuilder> configure = null)
        {
            //services.AddSingleton<IDataStorage,SqlServerStorage>();
            var builder = new DataStorageBuilder(services);
            configure?.Invoke(builder);

            return services;
        }

        public static DataStorageBuilder UseSqlServer(this DataStorageBuilder builder)
        {
            builder.Services.AddSingleton<IDataStorage, SqlServerStorage>();

            return builder;
        }

        #endregion

        #region Hoested Service
        public static IServiceCollection AddScheduleObserver(this IServiceCollection services)
        {
            services.AddHostedService<ScheduleObserverService>();
            return services;
        }

        #endregion

        #region Spider Scheduler
        public static IServiceCollection AddSpiderScheduler(this IServiceCollection services)
        {
            services.AddSingleton<ISpiderScheduler, ResourcesRestrictScheduler>();
            return services;
        }
        #endregion

        #region Quartz
        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            //scheduler.JobFactory = new SpiderJobFactory(services.BuildServiceProvider());
            services.AddSingleton(scheduler);
            //services.AddHostedService<QuartzHostedService>();

            return services;
        }
        #endregion

        #region Messsgae Queue

        public static IServiceCollection AddLocalMessagQueue(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus, LocalEventBus>();
            return services;
        }
        #endregion

        #region Cache
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddSingleton<ICache, ThreadSafeCache>();
            return services;
        }
        #endregion

        #region Statistics


        #endregion
    }
}
