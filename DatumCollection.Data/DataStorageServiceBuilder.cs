using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DatumCollection.Data
{
    public class DataStorageServiceBuilder
    {
        public IServiceCollection Services { get; }

        public DataStorageServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
