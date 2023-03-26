#if __IOS__
using SimpleWeather.Maui.BackgroundTasks;
#endif
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui
{
    public partial class App
    {
        private void App_OnCommonActionChanged(object sender, CommonActionChangedEventArgs e)
        {
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATETILELOCATION)
            {
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEAPI ||
                e.Action == CommonActions.ACTION_WEATHER_UPDATE)
            {
#if __IOS__
                WeatherUpdaterTask.UpdateWeather();
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEUNIT)
            {
#if __IOS__
                WidgetUpdaterTask.UpdateWidgets();
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEREFRESH ||
                e.Action == CommonActions.ACTION_WEATHER_REREGISTERTASK)
            {
#if __IOS__
                WeatherUpdaterTask.CancelPendingTasks();
                WeatherUpdaterTask.ScheduleTask();
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEGPS)
            {
                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;
            }
            else if (e.Action == CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE)
            {
                object forceUpdate = true;
                if (e.Extras?.TryGetValue(CommonActions.EXTRA_FORCEUPDATE, out forceUpdate) != true || (bool)forceUpdate)
                {
#if __IOS__
                    WeatherUpdaterTask.UpdateWeather();
#endif
                }

                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEDAILYNOTIFICATION)
            {
#if __IOS__
                if (SettingsManager.DailyNotificationEnabled)
                {
                    DailyNotificationTask.ScheduleTask();
                }
                else
                {
                    DailyNotificationTask.CancelPendingTasks();
                }
#endif
            }
            else if (e.Action == CommonActions.ACTION_LOCALE_CHANGED)
            {
                // Update locale for string resources
                UpdateAppLocale();
            }
        }
    }
}
