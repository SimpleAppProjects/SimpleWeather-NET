using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class AsyncTask
    {
        public static void Run(Action action, int millisDelay)
        {
            Task.Run(async () =>
            {
                await Task.Delay(millisDelay);

                action?.Invoke();
            });
        }

        public static void Run(Action action, int millisDelay, CancellationToken token)
        {
            Task.Run(async () =>
            {
                await Task.Delay(millisDelay);

                if (token.IsCancellationRequested)
                    return;

                action?.Invoke();
            });
        }
    }
}