using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core
{
    public class ServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
