using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DatumCollection.Core.Hosting
{
    internal interface IApplicationLifetime
    {
        /// <summary>
        /// triggered when the application is fully started and is about to wait
        /// for a graceful shutdown.
        /// </summary>
        CancellationToken ApplicationStarted { get; }

        /// <summary>
        /// triggered when the application host is performing a graceful shutdown.
        /// request may be still in flight, shutdown will block until the event completes.
        /// </summary>
        CancellationToken ApplicationStoppping { get; }

        /// <summary>
        /// triggered when the application host is performing a graceful shutdown .
        /// all requests should be complete at this point,shutdown will block until the event completes.
        /// </summary>
        CancellationToken ApplicationStopped { get; }

        /// <summary>
        /// requests termination of the current application
        /// </summary>
        void StopApplication();


    }
}
