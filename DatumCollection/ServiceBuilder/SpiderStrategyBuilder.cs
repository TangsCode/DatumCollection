using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ServiceBuilder
{
    public class SpiderStrategyBuilder
    {
        public IServiceCollection Services { get; set; }
        public SpiderStrategyBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
