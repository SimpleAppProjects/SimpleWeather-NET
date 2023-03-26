#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Maui.Notifications;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using UIKit;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class WidgetUpdaterTask : IBackgroundTask
    {
        private const string taskName = nameof(WidgetUpdaterTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private WidgetUpdaterTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            // Update config
            try
            {
                Logger.WriteLine(LoggerLevel.Debug, "{0}: Run...", taskName);

                if (SettingsManager.WeatherLoaded)
                {
                    // If saved data DNE (for current location), refresh weather
                    var result = await LoadWeather();
                    if (result is not WeatherResult.Success && result is not WeatherResult.WeatherWithError)
                    {
                        result = await LoadWeather(true);

                        if (result is WeatherResult.Success it && !it.IsSavedData)
                        {
                            // do something??
                        }
                    }

                    // Update widgets

                    if (SettingsManager.PoPChanceNotificationEnabled)
                    {
                        await PoPNotificationCreator.CreateNotification(await SettingsManager.GetHomeData()).WaitAsync(cts.Token);
                    }

                    // update shortcuts
                }

                Logger.WriteLine(LoggerLevel.Debug, "{0}: End of run...", taskName);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "{0}: exception occurred...", taskName);
            }
            finally
            {
                TaskCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private Task<WeatherResult> LoadWeather(bool forceRefresh = false)
        {
            return Task.Run<WeatherResult>(async () =>
            {
                Logger.WriteLine(LoggerLevel.Debug, "{0}: Getting weather data for home...", taskName);

                var locData = await SettingsManager.GetHomeData();

                if (locData == null)
                {
                    return new WeatherResult.Error(new WeatherException(WeatherUtils.ErrorStatus.NoWeather));
                }

                return await new WeatherDataLoader(locData).LoadWeatherResult(
                    new WeatherRequest.Builder().Apply(r =>
                    {
                        if (forceRefresh)
                        {
                            r.ForceRefresh(false)
                             .LoadAlerts()
                             .LoadForecasts();
                        }
                        else
                        {
                            r.ForceLoadSavedData();
                        }
                    }).Build()
                );
            });
        }

        public static void RegisterTask()
        {
            BGTaskScheduler.Shared.Register(TASK_ID, null, (task) =>
            {
                HandleTaskRegistration(task as BGAppRefreshTask);
            });
        }

        public static void UpdateWidgets()
        {
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Requesting to start work immediately", taskName);

            try
            {
                var cts = new CancellationTokenSource();
                var id = UIKit.UIApplication.SharedApplication.BeginBackgroundTask(cts.Cancel);

                if (id != UIApplication.BackgroundTaskInvalid)
                {
                    Task.Run(new WidgetUpdaterTask().Run, cts.Token).ContinueWith(t =>
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

        public static void CancelPendingTasks()
        {
            BGTaskScheduler.Shared.Cancel(TASK_ID);
        }

        private static void HandleTaskRegistration(BGAppRefreshTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new WidgetUpdaterTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        public static void ScheduleTask()
        {
            var request = new BGAppRefreshTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(TimeSpan.FromHours(1).TotalSeconds)
            };

            try
            {
                BGTaskScheduler.Shared.Submit(request, out var error);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: Unable to register task");
            }
        }
    }
}
#endif
