using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatumCollection.Core.Hosting
{
    /// <summary>
    /// 爬虫主机扩展
    /// </summary>
    public static class SpiderHostExtensions
    {
        public static void Run(this ISpiderHost host)
        {
            host.RunAsync().GetAwaiter().GetResult();
        }

        public static async Task RunAsync(this ISpiderHost host, CancellationToken token = default(CancellationToken))
        {
            // Wait for token shutdown if it can be canceled
            if (token.CanBeCanceled)
            {
                await host.RunAsync(token);
                return;
            }

            // If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    cts.Token.Register(() =>
                    {
                        host.Dispose();
                    });
                    Console.CancelKeyPress += (sender, arguments) => { cts.Cancel(); };                    
                }
                catch (Exception)
                {
                    
                }
                finally
                {
                    done.Set();
                }
            }
        }

        public static Task StopAsync(this ISpiderHost host, TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        public static void WaitForShutdown(this ISpiderHost host)
        {

        }

        public static Task WaitForShutdownAsync(this ISpiderHost host, CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}
