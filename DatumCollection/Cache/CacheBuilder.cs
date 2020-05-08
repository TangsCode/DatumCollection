using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Cache
{
    public class CacheBuilder
    {
        public IServiceCollection Services { get; }

        public CacheBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
