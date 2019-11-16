using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class AsyncTask
    {
        public static ConfiguredTaskAwaitable RunAsync(Func<Task> function)
        {
            return Task.Run(function).ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> RunAsync<T>(Func<T> function)
        {
            return Task.Run<T>(function).ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> RunAsync<T>(Func<Task<T>> function)
        {
            return Task.Run<T>(function).ConfigureAwait(false);
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

        public static Task RunOnUIThread(Action action)
        {
            return DispatcherHelper.ExecuteOnUIThreadAsync(action);
        }

        public static Task<T> RunOnUIThread<T>(Func<T> function)
        {
            return DispatcherHelper.ExecuteOnUIThreadAsync(function);
        }
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
