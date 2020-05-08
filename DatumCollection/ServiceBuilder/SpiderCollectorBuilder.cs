using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ServiceBuilder
{
    public class SpiderCollectorBuilder
    {
        public IServiceCollection Services { get; set; }

        public SpiderCollectorBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
