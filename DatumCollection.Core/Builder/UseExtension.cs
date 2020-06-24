using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatumCollection.Core.Builder
{
    /// <summary>
    /// Extension methods for adding middleware.
    /// </summary>
    public static class UseExtension
    {
        /// <summary>
        /// adds a middlware delegate to the defined in-line to the application's spider pipline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public static IApplicationBuilder Use(this IApplicationBuilder app,Func<SpiderContext, Func<Task>,Task> middleware)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }
    }
}
