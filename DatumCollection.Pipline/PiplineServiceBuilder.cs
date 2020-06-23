using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Pipline
{
    public class PiplineServiceBuilder
    {
        public IServiceCollection Services { get; }

        public PiplineServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
