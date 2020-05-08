using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatumCollection.Logger
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LoggerExtensions
    {
        public static void LogSpiderError(this ILogger logger,Exception e)
        {
            logger.LogError(e, null, null);
        }

        public static void LogSpiderError(this ILogger logger,string message,object[] args = null)
        {
            logger.LogError(message, args);
        }
    }
}
