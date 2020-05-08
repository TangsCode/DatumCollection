using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ServiceBuilder
{
    public class StatisticsBuilder
    {
        public IServiceCollection Services { get; }

        public StatisticsBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
