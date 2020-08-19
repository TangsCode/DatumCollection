using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Middleware
{
    public class MiddlewareFactory : IMiddlewareFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MiddlewareFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMiddleware Create(Type middlewareType)
        {
            return _serviceProvider.GetRequiredService(middlewareType) as IMiddleware;
        }

        public void Release(IMiddleware middleware)
        {
            
        }
    }
}
