using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
                await host.RunAsync(token, startupMessage: null);
                return;
            }

            // If token cannot be canceled, attach Ctrl+C and SIGTERM shutdown
            var done = new ManualResetEventSlim(false);
            using (var cts = new CancellationTokenSource())
            {
                var shutdownMessage = "spider host is shutting down...";
                using (var lifetime = new SpiderHostLifetime(cts, done, shutdownMessage))
                {
                    try
                    {
                        await host.RunAsync(cts.Token, "spider host is starting, please Press CTRL+C to shut down.");
                        lifetime.SetExitGracefully();
                    }
                    finally
                    {
                        done.Set();
                    }
                }
            }
        }

        public static async Task RunAsync(this ISpiderHost host,CancellationToken token, string startupMessage)
        {
            try
            {
                Console.WriteLine(startupMessage);
                await host.StartAsync(token);                
                await host.WaitForShutdownAsync(token);
            }
            finally
            {
                host?.Dispose();
            }
        }

        public static Task StopAsync(this ISpiderHost host, TimeSpan timeout)
        {
            return host.StopAsync(new CancellationTokenSource(timeout).Token);
        }

        public static void WaitForShutdown(this ISpiderHost host)
        {
            host.WaitForShutdownAsync().GetAwaiter().GetResult();
        }

        public static async Task WaitForShutdownAsync(this ISpiderHost host, CancellationToken token = default(CancellationToken))
        {
            var applicationLifetime = host.Services.GetRequiredService<ApplicationLifetime>();
            token.Register(state =>
            {
                ((IApplicationLifetime)state).StopApplication();
            },
            applicationLifetime);

            var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            applicationLifetime.ApplicationStoppping.Register(obj =>
            {
                var tcs = (TaskCompletionSource<object>)obj;
                tcs.TrySetResult(null);
            }, waitForStop);

            await waitForStop.Task;

            await host.StopAsync();
        }

        public static ISpiderHostBuilder UseStartUp<TStartup>(this ISpiderHostBuilder spiderHostBuilder) where TStartup : class
        {
            return spiderHostBuilder.UseStartup(typeof(TStartup));
        }

        public static ISpiderHostBuilder UseStartup(this ISpiderHostBuilder spiderHostBuilder,Type startupType)
        {
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;

            return spiderHostBuilder.ConfigureServices(services => {
                if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startupType.GetTypeInfo()))
                {
                    services.AddSingleton(typeof(IStartup), startupType);
                }                
            });            
        }
    }
}
