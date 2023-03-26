#if __IOS__ || __MACCATALYST__
using System;
using System.Threading.Tasks;

namespace SimpleWeather.BackgroundTasks
{
    public interface IBackgroundTask
    {
        public Task Run();
        public void Cancel();
        public bool IsCancelled { get; }
        public event EventHandler TaskCompleted;
    }
}
#endif