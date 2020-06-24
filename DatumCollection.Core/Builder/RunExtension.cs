using DatumCollection.Infrastructure.Spider;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public static class RunExtension
    {
        public static void Run(this IApplicationBuilder app,PiplineDelegate handler)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            app.Use(_ => handler);
        }
    }
}
