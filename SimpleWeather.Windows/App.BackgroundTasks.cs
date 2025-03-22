using SimpleWeather.Extras.BackgroundTasks;
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.Utils;
using Windows.ApplicationModel.Background;
using WinRT;

namespace SimpleWeather.NET
{
    public partial class App
    {
        private static uint _InProcRegistrationToken;

        private static void RegisterInProcBackgroundTask()
        {
            var inProcBackgroundTaskFactory = new ClassFactory<InProcBackgroundTask>(
                () => new InProcBackgroundTask(),
                new Dictionary<Guid, Func<object, IntPtr>>()
                {
                    { typeof(IBackgroundTask).GUID, obj => MarshalInterface<IBackgroundTask>.FromManaged((IBackgroundTask)obj) },
                });

            // On launch register the BackgroundTask class for OOP COM activation
            _InProcRegistrationToken = COMUtilities.RegisterClass<InProcBackgroundTask>(inProcBackgroundTaskFactory);
        }

        private static void UnregisterInProcBackgroundTask()
        {
            COMUtilities.UnregisterClassObject(_InProcRegistrationToken);
        }

        internal async Task OnBackgroundActivatedAsync(IBackgroundTaskInstance instance)
        {
            await TaskEx.YieldToBackground();

            Initialize(instance);

            AnalyticsLogger.LogEvent("App: Background Activated",
                new Dictionary<string, string>()
                {
                    { "Task", instance.Task?.Name }
                });

            switch (instance.Task?.Name)
            {
                case nameof(WeatherUpdateBackgroundTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting WeatherUpdateBackgroundTask");
                    new WeatherUpdateBackgroundTask().Run(instance);
                    break;

                case nameof(UpdateTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting UpdateTask");
                    new UpdateTask().Run(instance);
                    break;

                case nameof(AppUpdaterTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting AppUpdaterTask");
                    new AppUpdaterTask().Run(instance);
                    break;

                case nameof(RemoteConfigUpdateTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting RemoteConfigUpdateTask");
                    new RemoteConfigUpdateTask().Run(instance);
                    break;

                case nameof(WeatherTileUpdaterTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting WeatherTileUpdaterTask");
                    new WeatherTileUpdaterTask().Run(instance);
                    break;

                case nameof(DailyNotificationTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting DailyNotificationTask");
                    new DailyNotificationTask().Run(instance);
                    break;

                case nameof(PremiumStatusTask):
                    Logger.WriteLine(LoggerLevel.Debug, "App: Starting PremiumStatusTask");
                    new PremiumStatusTask().Run(instance);
                    break;

                default:
                    Logger.WriteLine(LoggerLevel.Debug, "App: Unknown task: {0}", instance.Task?.Name);
                    break;
            }
        }
    }
}
