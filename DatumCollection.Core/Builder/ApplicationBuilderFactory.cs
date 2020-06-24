using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public class ApplicationBuilderFactory : IApplicationBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IApplicationBuilder CreateBuilder()
        {
            return new ApplicationBuilder(_serviceProvider);
        }
    }
}
