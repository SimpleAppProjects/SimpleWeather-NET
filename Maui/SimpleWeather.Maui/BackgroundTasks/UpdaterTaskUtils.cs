using CommunityToolkit.Mvvm.DependencyInjection;
#if __IOS__
using SimpleWeather.Maui.Widget;
#endif
using SimpleWeather.Preferences;

namespace SimpleWeather.Maui.BackgroundTasks
{
    public static class UpdaterTaskUtils
    {
        public static void StartTasks()
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();
            // Queue tasks if dependent features are enabled
            Task.Run(async () =>
            {
                if (await IsTaskFeaturesEnabled())
                {
#if __IOS__
                    WidgetUpdaterTask.ScheduleTask();
                    WeatherUpdaterTask.ScheduleTask();

                    if (settingsMgr.DailyNotificationEnabled)
                    {
                        EnableDailyNotificationTask(true);
                    }
#endif
                }
            });
        }

        public static void CancelTasks()
        {
            // Cancel tasks if dependent features are disabled
            Task.Run(async () =>
            {
                if (!await IsTaskFeaturesEnabled())
                {
#if __IOS__
                    WidgetUpdaterTask.CancelPendingTasks();
                    WeatherUpdaterTask.CancelPendingTasks();
#endif
                }
            });
        }

        public static void UpdateTasks()
        {
#if __IOS__
            WidgetUpdaterTask.CancelPendingTasks();
            WidgetUpdaterTask.ScheduleTask();

            WeatherUpdaterTask.CancelPendingTasks();
            WeatherUpdaterTask.ScheduleTask();
#endif
        }

        public static void EnableDailyNotificationTask(bool enable)
        {
#if __IOS__
            if (enable)
            {
                DailyNotificationTask.ScheduleTask();
            }
            else
            {
                DailyNotificationTask.CancelPendingTasks();
            }

            if (!enable) CancelTasks();
#endif
        }

        public static void RescheduleDailyNotificationTask()
        {
#if __IOS__
            DailyNotificationTask.ScheduleTask();
#endif
        }

        private static async Task<bool> IsTaskFeaturesEnabled()
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();
            return
#if __IOS__
                await WeatherWidgetUpdater.WidgetsExist() ||
#endif
                settingsMgr.ShowAlerts ||
                settingsMgr.DailyNotificationEnabled ||
                settingsMgr.PoPChanceNotificationEnabled;
        }
    }
}
