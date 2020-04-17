using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SimpleWeather.Utils
{
    public static class AsyncTask
    {
        public static ConfiguredTaskAwaitable CreateTask(Func<Task> function)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));
            return function.Invoke().ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> CreateTask<T>(Func<Task<T>> function)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));
            return function.Invoke().ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable RunAsync(Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> RunAsync<T>(Task<T> task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable RunAsync(ValueTask task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable<T> RunAsync<T>(ValueTask<T> task)
        {
            return task.ConfigureAwait(false);
        }

        public static void Run(Action action)
        {
            Task.Run(action);
        }

        public static void Run(Action action, CancellationToken token)
        {
            Task.Run(action, token);
        }

        public static void Run(Action action, int millisDelay)
        {
            Task.Run(() =>
            {
                Task.Delay(millisDelay);

                action?.Invoke();
            });
        }

        public static void Run(Action action, int millisDelay, CancellationToken token)
        {
            Task.Run(() =>
            {
                Task.Delay(millisDelay);

                if (token.IsCancellationRequested)
                    return;

                action?.Invoke();
            });
        }

        public static Task RunOnUIThread(this CoreDispatcher Dispatcher, Action action)
        {
            if (Dispatcher.HasThreadAccess)
            {
                action?.Invoke();
                return Task.CompletedTask;
            }
            else
            {
                return Dispatcher.AwaitableRunAsync(action);
            }
        }

        public static Task<T> RunOnUIThread<T>(this CoreDispatcher Dispatcher, Func<T> function)
        {
            if (Dispatcher.HasThreadAccess)
            {
                return Task.FromResult(function.Invoke());
            }
            else
            {
                return Dispatcher.AwaitableRunAsync(function);
            }
        }

        private static CoreDispatcher GetDispatcher()
        {
            CoreDispatcher Dispatcher = null;

            try
            {
                try
                {
                    Dispatcher = CoreApplication.MainView?.Dispatcher;
                }
                catch (Exception) { }

                if (Dispatcher == null)
                {
                    Dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Dispatcher unavailable");
            }

            return Dispatcher;
        }

        public static Task TryRunOnUIThread(Action action)
        {
            var Dispatcher = GetDispatcher();

            if (Dispatcher != null)
            {
                if (Dispatcher.HasThreadAccess)
                {
                    action?.Invoke();
                    return Task.CompletedTask;
                }
                else
                {
                    return Dispatcher.AwaitableRunAsync(action);
                }
            }
            else
            {
                // Dispatcher is not available
                return Task.CompletedTask;
            }
        }
    }
}