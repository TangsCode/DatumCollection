using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.ServiceBuilder
{
    public class RecognizerBuilder
    {
        public IServiceCollection Services { get; set; }

        public RecognizerBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
