using DatumCollection.Exceptions;
using DatumCollection.Spiders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection
{
    /// <summary>
    /// Services provider those are registered before
    /// </summary>
    public class ServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public AbstractSpider CreateSpider<T>()
        {
            if (!typeof(AbstractSpider).IsAssignableFrom(typeof(T)))
            {
                throw new SpiderException($"{typeof(T)} is not an implementation of AbstractSpider");
            }

            return (AbstractSpider)_serviceProvider.GetRequiredService(typeof(T));
        }

        public T GetRequiredService<T>()
        {
            return (T)_serviceProvider.GetRequiredService<T>();
        }

        public IServiceProvider CreateScopedServiceProvider()
        {
            return _serviceProvider.CreateScope().ServiceProvider;
        }
    }
}
