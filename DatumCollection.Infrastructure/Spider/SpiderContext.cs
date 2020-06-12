using System;

namespace DatumCollection.Infrastructure.Spider
{
    public class SpiderContext
    {
        public SpiderTask Task { get; set; }

        public IServiceProvider Services { get; }

        public SpiderContext(
            IServiceProvider services)
        {
            Services = services;
        }
    }
    
}
