using SimpleWeather.Preferences;
using SimpleWeather.Utils;
#if WINDOWS
using SimpleWeather.Uno.BackgroundTasks;
using SimpleWeather.Uno.Tiles;
#endif
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWeather.Uno
{
    public sealed partial class App
    {
        private async void App_OnCommonActionChanged(object sender, CommonActionChangedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATETILELOCATION)
            {
#if WINDOWS
                if (e.Extras?.ContainsKey(Constants.TILEKEY_OLDKEY) == true)
                {
                    string oldKey = e.Extras?[Constants.TILEKEY_OLDKEY]?.ToString();
                    string locationQuery = e.Extras?[Constants.TILEKEY_LOCATION]?.ToString();

                    if (SecondaryTileUtils.Exists(oldKey))
                    {
                        SecondaryTileUtils.UpdateTileId(oldKey, locationQuery);
                    }
                }
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEAPI ||
                e.Action == CommonActions.ACTION_WEATHER_UPDATE)
            {
#if WINDOWS
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEUNIT)
            {
#if WINDOWS
                await Task.Run(WeatherTileUpdaterTask.RequestAppTrigger);
#endif
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEREFRESH ||
                e.Action == CommonActions.ACTION_WEATHER_REREGISTERTASK)
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
                    if (prevLoc?.query != null && SecondaryTileUtils.Exists(prevLoc.query))
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
                }
                else
                {
                    if (SecondaryTileUtils.Exists(Constants.KEY_GPS))
                    {
                        var favLoc = (await SettingsManager.GetFavorites()).FirstOrDefault();
                        if (favLoc?.IsValid() == true)
                            SecondaryTileUtils.UpdateTileId(Constants.KEY_GPS, favLoc.query);
                    }
                }
#endif

                // Reset notification time for new location
                SettingsManager.LastPoPChanceNotificationTime = DateTimeOffset.MinValue;
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
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
