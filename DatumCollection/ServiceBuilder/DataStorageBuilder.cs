using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace DatumCollection.ServiceBuilder
{
    public class DataStorageBuilder
    {
        public IServiceCollection Services;
        public DataStorageBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
