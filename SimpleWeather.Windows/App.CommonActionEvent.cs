using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.NET.Widgets;
using Windows.UI.StartScreen;
#if WINDOWS
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.NET.Tiles;
#endif

namespace SimpleWeather.NET
{
    public sealed partial class App
    {
        private async void App_OnCommonActionChanged(object sender, CommonActionChangedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATEWIDGETLOCATION)
            {
#if WINDOWS
                if (e.Extras?.ContainsKey(Constants.WIDGETKEY_OLDKEY) == true)
                {
                    string oldKey = e.Extras?[Constants.WIDGETKEY_OLDKEY]?.ToString();
                    object locationQuery = e.Extras?[Constants.WIDGETKEY_LOCATION];

                    if (SecondaryTileUtils.Exists(oldKey))
                    {
                        if (locationQuery is LocationData.LocationData locData)
                        {
                            SecondaryTileUtils.UpdateTileId(oldKey, locData.query);
                        }
                        else
                        {
                            SecondaryTileUtils.UpdateTileId(oldKey, Constants.KEY_GPS);
                        }
                    }
                    if (WidgetUtils.Exists(oldKey))
                    {
                        if (locationQuery is LocationData.LocationData locData)
                        {
                            WidgetUtils.UpdateWidgetIds(oldKey, locData.query);
                        }
                        else
                        {
                            WidgetUtils.UpdateWidgetIds(oldKey, Constants.KEY_GPS);
                        }
                    }
                }
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEAPI)
            {
#if WINDOWS
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEUNIT)
            {
#if WINDOWS
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEREFRESH)
            {
#if WINDOWS
                await Task.Run(() => WeatherUpdateBackgroundTask.RegisterBackgroundTask(true));
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEGPS)
            {
#if WINDOWS
                // Update tile ids when switching GPS feature
                if (SettingsManager.FollowGPS)
                {
                    var prevLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                    if (prevLoc?.query != null)
                    {
                        if (SecondaryTileUtils.Exists(prevLoc.query))
                        {
                            var gpsLoc = await SettingsManager.GetLastGPSLocData();
                            if (gpsLoc?.query == null)
                            {
                                await SettingsManager.SaveLastGPSLocData(prevLoc);
                            }
                            else
                            {
                                SecondaryTileUtils.UpdateTileId(prevLoc.query, Constants.KEY_GPS);
                            }
                        }
                        if (WidgetUtils.Exists(prevLoc.query))
                        {
                            var gpsLoc = await SettingsManager.GetLastGPSLocData();
                            if (gpsLoc?.query == null)
                            {
                                await SettingsManager.SaveLastGPSLocData(prevLoc);
                            }
                            else
                            {
                                WidgetUtils.UpdateWidgetIds(prevLoc.query, Constants.KEY_GPS);
                            }
                        }
                    }
                }
                else
                {
                    if (SecondaryTileUtils.Exists(Constants.KEY_GPS))
                    {
                        var favLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                        if (favLoc?.IsValid() == true)
                            SecondaryTileUtils.UpdateTileId(Constants.KEY_GPS, favLoc.query);
                    }
                    if (WidgetUtils.Exists(Constants.KEY_GPS))
                    {
                        var favLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                        if (favLoc?.IsValid() == true)
                            WidgetUtils.UpdateWidgetIds(Constants.KEY_GPS, favLoc);
                    }
                }
#endif

                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;

                // Update tiles and widgets
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
            }
            else if (e.Action == CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE)
            {
                object forceUpdate = true;
                if (e.Extras?.TryGetValue(CommonActions.EXTRA_FORCEUPDATE, out forceUpdate) != true || (bool)forceUpdate)
                {
#if WINDOWS
                    await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
#endif
                }

                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEDAILYNOTIFICATION)
            {
#if WINDOWS
                if (SettingsManager.DailyNotificationEnabled)
                {
                    Task.Run(() => DailyNotificationTask.RegisterBackgroundTask(false));
                }
                else
                {
                    Task.Run(() => DailyNotificationTask.UnregisterBackgroundTask());
                }
#endif
            }
            else if (e.Action == CommonActions.ACTION_LOCALE_CHANGED)
            {
                // Update locale for string resources
                UpdateAppLocale();
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
            }
            else if (e.Action == CommonActions.ACTION_WEATHER_LOCATIONREMOVED)
            {
                // Update widgets accordingly
                var query = e.Extras[Constants.WIDGETKEY_LOCATIONQUERY] as string;

                // Remove secondary tile if it exists
                if (SecondaryTileUtils.Exists(query))
                {
                    await new SecondaryTile(
                        SecondaryTileUtils.GetTileId(query)).RequestDeleteAsync();
                }
                if (WidgetUtils.Exists(query))
                {
                    WidgetUtils.GetWidgetIds(query).ForEach(id =>
                    {
                        WidgetUtils.DeleteWidget(id);
                    });
                }
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
