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
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATEWIDGETLOCATION)
            {
                // no-op
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEAPI)
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
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEREFRESH)
            {
#if __IOS__
                UpdaterTaskUtils.UpdateTasks();
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEGPS)
            {
                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;

                // Update widgets
                WidgetUpdaterTask.UpdateWidgets();
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
            else if (e.Action == CommonActions.ACTION_WEATHER_LOCATIONREMOVED)
            {
                // Update widgets accordingly
                var query = e.Extras[Constants.WIDGETKEY_LOCATIONQUERY] as string;
#if __IOS__
                // Remove query from weather json map
#endif
            }
        }
    }
}
