using Microsoft.UI.Dispatching;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace SimpleWeather.NET
{
    /// <summary>
    /// DispatcherQueueSyncContext allows developers to await calls and get back onto the
    /// UI thread. Needs to be installed on the UI thread through DispatcherQueueSyncContext.SetForCurrentThread
    ///
    /// This class has been modified from the one shipping in the WindowsAppSDK by adding support for Send, see
    /// https://github.com/microsoft/CsWinRT/pull/1078
    /// </summary>
    public class DispatcherQueueSynchronizationContext : SynchronizationContext
    {
        private readonly DispatcherQueue dispatcherQueue;

        public DispatcherQueueSynchronizationContext(DispatcherQueue dispatcherQueue)
        {
            this.dispatcherQueue = dispatcherQueue;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            dispatcherQueue.TryEnqueue(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (dispatcherQueue.HasThreadAccess)
            {
                d(state);
            }
            else
            {
                var m = new ManualResetEvent(false);
                ExceptionDispatchInfo edi = null;

                dispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        d(state);
                    }
                    catch (Exception ex)
                    {
                        edi = ExceptionDispatchInfo.Capture(ex);
                    }
                    finally
                    {
                        m.Set();
                    }
                });
                m.WaitOne();

#pragma warning disable CA1508 // Avoid dead conditional code
                edi?.Throw();
#pragma warning restore CA1508 // Avoid dead conditional code
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return new DispatcherQueueSynchronizationContext(dispatcherQueue);
        }
    }
}
