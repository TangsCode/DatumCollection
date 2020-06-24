using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Core.Hosting
{
    public static class HostingLoggerExtensions
    {
        public static void Starting(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("host is starting");
            }
        }

        public static void Started(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("host started");
            }
        }

        public static void Shutdown(this ILogger logger)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("host shutdown");
            }
        }

        public static void HostingStartupAssemblyError(this ILogger logger, Exception exception)
        {
            logger.LogError("Hosting startup assembly exception :{0}", exception);
        }
        
    }
}
