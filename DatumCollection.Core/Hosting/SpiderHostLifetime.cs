using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatumCollection.Core.Hosting
{
    /// <summary>
    /// 爬虫托管服务生命周期
    /// </summary>
    internal class SpiderHostLifetime : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly ManualResetEventSlim _resetEvent;
        private readonly string _shutdownMessage;

        private bool _disposed = false;
        private bool _exeitedGracefully;

        public SpiderHostLifetime(CancellationTokenSource cts,ManualResetEventSlim resetEvent,string shutdownMessage)
        {
            _cts = cts;
            _resetEvent = resetEvent;
            _shutdownMessage = shutdownMessage;

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            Console.CancelKeyPress += CancelKeyPress;
        }

        internal void SetExitGracefully()
        {
            _exeitedGracefully = true;
        }

        private void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Shutdown();
            //donn't terminate the process immediately,wait for the main thread to exit gracefully.
            e.Cancel = true;
        }

        private void ProcessExit(object sender, EventArgs e)
        {
            Shutdown();
            if (_exeitedGracefully)
            {
                // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
                Environment.ExitCode = 0;
            }
        }

        private void Shutdown()
        {
            try
            {
                if (!_cts.IsCancellationRequested)
                {
                    if (!string.IsNullOrEmpty(_shutdownMessage))
                    {
                        Console.WriteLine(_shutdownMessage);
                    }
                    _cts.Cancel();
                }
            }
            catch (Exception)
            {
            }
            _resetEvent.Wait();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            Console.CancelKeyPress -= CancelKeyPress;
        }
    }
}
