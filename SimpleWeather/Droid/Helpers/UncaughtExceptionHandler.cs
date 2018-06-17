#if __ANDROID__

using Java.Lang;

namespace SimpleWeather.Droid.Helpers
{
    public class UncaughtExceptionHandler : Object, Thread.IUncaughtExceptionHandler
    {
        private System.Action<Thread, Throwable> ExceptionHandler;

        public UncaughtExceptionHandler(System.Action<Thread, Throwable> ExceptionHandler)
        {
            this.ExceptionHandler = ExceptionHandler;
        }

        public void UncaughtException(Thread t, Throwable e)
        {
            ExceptionHandler?.Invoke(t, e);
        }
    }
}
#endif