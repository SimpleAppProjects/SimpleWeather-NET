#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Maui.Images;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using UIKit;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class FCMTask : IBackgroundTask
    {
        private const string taskName = nameof(FCMTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";

        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private FCMTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                // Check if cache is populated
                var imageDataService = Ioc.Default.GetService<IImageDataService>();
                if (!await imageDataService.IsEmpty() && !UpdateSettings.IsUpdateAvailable)
                {
                    // If so, check if we need to invalidate
                    var updateTime = 0L;
                    try
                    {
                        updateTime = await ImageDatabase.GetLastUpdateTime();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                    }

                    if (updateTime > imageDataService.GetImageDBUpdateTime())
                    {
                        AnalyticsLogger.LogEvent($"{taskName}: clearing image cache");

                        // if so, invalidate
                        imageDataService.SetImageDBUpdateTime(updateTime);
                        await imageDataService.ClearCachedImageData();
                        imageDataService.InvalidateCache(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
            finally
            {
                TaskCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public static void InvalidateCache()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            try
            {
                var cts = new CancellationTokenSource();
                var id = UIKit.UIApplication.SharedApplication.BeginBackgroundTask($"{TASK_ID}.immediate", cts.Cancel);

                if (id != UIApplication.BackgroundTaskInvalid)
                {
                    Task.Run(new FCMTask().Run, cts.Token).ContinueWith(t =>
                    {
                        UIApplication.SharedApplication.EndBackgroundTask(id);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }
    }
}
#endif
