using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public class ServiceBuilder
    {
        public IServiceCollection Services { get; }

        public ServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
