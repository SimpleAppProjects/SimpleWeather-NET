#if __IOS__ || __MACCATALYST__
using BackgroundTasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.BackgroundTasks;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public class RemoteConfigUpdateTask : IBackgroundTask
    {
        private const string taskName = nameof(RemoteConfigUpdateTask);
        public const string TASK_ID = $"SimpleWeather.{taskName}";
        private static bool Registered = false;

        private readonly CancellationTokenSource cts = new();

        public bool IsCancelled => cts.IsCancellationRequested;

        public event EventHandler TaskCompleted;

        private RemoteConfigUpdateTask() { }

        public void Cancel()
        {
            cts.Cancel();
        }

        public async Task Run()
        {
            try
            {
                // Update config
                var remoteConfigService = Ioc.Default.GetService<IRemoteConfigService>();
                await remoteConfigService.CheckConfig();
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

        public static void RegisterTask()
        {
            if (!Registered)
            {
                Registered = BGTaskScheduler.Shared.Register(TASK_ID, null, (task) =>
                {
                    HandleTaskRegistration(task as BGProcessingTask);
                });
            }
            else
            {
                ScheduleTask();
            }
        }

        public static void CancelPendingTasks()
        {
            BGTaskScheduler.Shared.Cancel(TASK_ID);
        }

        private static void HandleTaskRegistration(BGProcessingTask task)
        {
            // Schedule a new refresh task.
            ScheduleTask();

            var taskOp = new RemoteConfigUpdateTask();

            task.ExpirationHandler = taskOp.Cancel;

            taskOp.TaskCompleted += (s, e) =>
            {
                task.SetTaskCompleted(!taskOp.IsCancelled);
            };

            Task.Run(taskOp.Run);
        }

        private static void ScheduleTask()
        {
            var request = new BGProcessingTaskRequest(TASK_ID)
            {
                EarliestBeginDate = Foundation.NSDate.FromTimeIntervalSinceNow(TimeSpan.FromDays(1).TotalSeconds),
                RequiresNetworkConnectivity = true
            };

            try
            {
                BGTaskScheduler.Shared.Submit(request, out var error);

                if (error != null)
                {
                    Logger.WriteLine(LoggerLevel.Error, $"{taskName}: Error - ${error}");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, $"{taskName}: Unable to register task");
            }
        }
    }
}
#endif
