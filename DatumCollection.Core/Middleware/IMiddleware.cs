using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Middleware
{
    /// <summary>
    /// middleware interface
    /// </summary>
    public interface IMiddleware
    {
        Task InvokeAsync(SpiderContext context, PiplineDelegate next);
    }
}
