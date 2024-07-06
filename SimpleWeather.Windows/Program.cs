using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace SimpleWeather.NET
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = DecideRedirection();
            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
        }

        private static bool DecideRedirection()
        {
            bool isRedirect = false;

            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();

            try
            {
                AppInstance keyInstance = AppInstance.FindOrRegisterForKey(
#if DEBUG
                    "SimpleWeather_Debug.Windows.app"
#else
                    "SimpleWeather.Windows.app"
#endif
                    );

                if (!keyInstance.IsCurrent)
                {
                    isRedirect = true;
                    RedirectActivationTo(args, keyInstance);
                }
            }
            catch { }

            return isRedirect;
        }

        // Do the redirection on another thread, and use a non-blocking
        // wait method to wait for the redirection to complete.
        public static void RedirectActivationTo(AppActivationArguments args, AppInstance keyInstance)
        {
            var redirectSemaphore = new Semaphore(0, 1);
            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                redirectSemaphore.Release();
            });
            redirectSemaphore.WaitOne();
        }
    }
}
