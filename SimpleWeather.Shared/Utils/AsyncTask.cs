using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
    }
}