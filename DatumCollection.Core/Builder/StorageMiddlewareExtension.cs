﻿using DatumCollection.Core.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Builder
{
    public static class StorageMiddlewareExtension
    {
        public static IApplicationBuilder UseStorage(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<StorageMiddleware>();
        }
    }
}
