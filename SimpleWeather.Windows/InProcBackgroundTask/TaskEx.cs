using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.NET
{
    public static class TaskEx
    {
#pragma warning disable CA1815, CA1822, CA1034, S3898 // Type only used for specific await pattern
        /// <summary>Provides an awaitable context for switching into a target environment.</summary>
        public readonly struct YieldAwaitable
        {
            private readonly SynchronizationContext syncContext;

            public YieldAwaitable(SynchronizationContext syncContext)
            {
                this.syncContext = syncContext;
            }

            /// <summary>Gets an awaiter for this <see cref="YieldAwaitable"/>.</summary>
            /// <returns>An awaiter for this awaitable.</returns>
            /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
            public YieldAwaiter GetAwaiter()
            {
                return new YieldAwaiter(this.syncContext);
            }

            /// <summary>Provides an awaiter that switches into a target environment.</summary>
            /// <remarks>This type is intended for compiler use only.</remarks>
            public readonly struct YieldAwaiter : ICriticalNotifyCompletion
            {
                private readonly SynchronizationContext syncContext;

                public YieldAwaiter(SynchronizationContext syncContext)
                {
                    this.syncContext = syncContext;
                }

                /// <summary>Gets whether a yield is not required.</summary>
                /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
                public bool IsCompleted => false;  // yielding is always required for YieldAwaiter, hence false

                /// <summary>Posts the <paramref name="continuation"/> back to the current context.</summary>
                /// <param name="continuation">The action to invoke asynchronously.</param>
                /// <exception cref="System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
                public void OnCompleted(Action continuation)
                {
                    QueueContinuation(continuation, this.syncContext, flowContext: true);
                }

                /// <summary>Posts the <paramref name="continuation"/> back to the current context.</summary>
                /// <param name="continuation">The action to invoke asynchronously.</param>
                /// <exception cref="System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
                public void UnsafeOnCompleted(Action continuation)
                {
                    QueueContinuation(continuation, this.syncContext, flowContext: false);
                }

                /// <summary>Posts the <paramref name="continuation"/> back to the current context.</summary>
                /// <param name="continuation">The action to invoke asynchronously.</param>
                /// <param name="flowContext">true to flow ExecutionContext; false if flowing is not required.</param>
                /// <exception cref="System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
                private static void QueueContinuation(Action continuation, SynchronizationContext syncContext, bool flowContext)
                {
                    // Validate arguments
                    if (continuation == null)
                    {
                        throw new ArgumentNullException(nameof(continuation));
                    }

                    // Get the current SynchronizationContext, and if there is one,
                    // post the continuation to it.  However, treat the base type
                    // as if there wasn't a SynchronizationContext, since that's what it
                    // logically represents.
                    if (syncContext != null && syncContext.GetType() != typeof(SynchronizationContext))
                    {
                        syncContext.Post(SendOrPostCallbackRunAction, continuation);
                    }
                    else
                    {
                        // If we're targeting the default scheduler, queue to the thread pool, so that we go into the global
                        // queue.  As we're going into the global queue, we might as well use QUWI, which for the global queue is
                        // just a tad faster than task, due to a smaller object getting allocated and less work on the execution path.
                        TaskScheduler scheduler = TaskScheduler.Current;
                        if (scheduler == TaskScheduler.Default)
                        {
                            if (flowContext)
                            {
                                ThreadPool.QueueUserWorkItem(WaitCallbackRunAction, continuation);
                            }
                            else
                            {
                                ThreadPool.UnsafeQueueUserWorkItem(WaitCallbackRunAction, continuation);
                            }
                        }

                        // We're targeting a custom scheduler, so queue a task.
                        else
                        {
                            Task.Factory.StartNew(continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                        }
                    }
                }

                /// <summary>
                /// WaitCallback that invokes the Action supplied as object state.
                /// </summary>
                private static readonly WaitCallback WaitCallbackRunAction = RunAction;

                /// <summary>
                /// SendOrPostCallback that invokes the Action supplied as object state.
                /// </summary>
                private static readonly SendOrPostCallback SendOrPostCallbackRunAction = RunAction;

                /// <summary>
                /// Runs an Action delegate provided as state.
                /// </summary>
                /// <param name="state">The Action delegate to invoke.</param>
                private static void RunAction(object state)
                {
                    ((Action)state)();
                }

                /// <summary>Ends the await operation.</summary>
                public void GetResult()
                {
                    // Nop. It exists purely because the compiler pattern demands it.
                }
            }
        }
#pragma warning restore CA1815, CA1822, CA1034, S3898

        /// <summary>
        /// Creates an awaitable that asynchronously yields back to the TaskScheduler.Current context when awaited.
        /// </summary>
        public static YieldAwaitable YieldToBackground()
        {
            return new YieldAwaitable(null);
        }

        public static YieldAwaitable YieldToContext(SynchronizationContext synchronizationContext)
        {
            return new YieldAwaitable(synchronizationContext);
        }
    }
}
