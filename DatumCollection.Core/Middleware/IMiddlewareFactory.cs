using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Middleware
{
    public interface IMiddlewareFactory
    {
        IMiddleware Create(Type middlewareType);

        void Release(IMiddleware middleware);
    }
}
