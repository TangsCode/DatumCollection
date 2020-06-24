using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public interface IApplicationBuilder
    {
        /// <summary>
        /// gets or sets the <see cref="IServiceProvider"/> that provides access to the application's service container.
        /// </summary>
        IServiceProvider ApplicationServices { get; set; }

        /// <summary>
        /// gets a key/value collection that can be used to share data between middleware.
        /// </summary>
        IDictionary<string,object> Properties { get; }

        /// <summary>
        /// add a miidleware delegate to the application's spider pipline.
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        IApplicationBuilder Use(Func<PiplineDelegate, PiplineDelegate> middleware);

        /// <summary>
        /// creates a new <see cref="IApplicationBuilder"/> that shares the <see cref="Properties"/> of this
        /// <see cref="IApplicationBuilder"/>
        /// </summary>
        /// <returns></returns>
        IApplicationBuilder New();

        /// <summary>
        /// builds the delegate used by the application to process <see cref="SpiderContext"/>
        /// </summary>
        /// <returns></returns>
        PiplineDelegate Build();
    }
}
