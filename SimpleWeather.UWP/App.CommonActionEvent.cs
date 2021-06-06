using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP
{
    public sealed partial class App
    {
        public async void App_OnCommonActionChanged(object sender, CommonActionChangedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATETILELOCATION)
            {
                if (e.Extras?.ContainsKey(Constants.TILEKEY_OLDKEY) == true)
                {
                    string oldKey = e.Extras?[Constants.TILEKEY_OLDKEY]?.ToString();
                    string locationQuery = e.Extras?[Constants.TILEKEY_LOCATION]?.ToString();

                    if (SecondaryTileUtils.Exists(oldKey))
                    {
                        SecondaryTileUtils.UpdateTileId(oldKey, locationQuery);
                    }
                }
            }
            else if (e.Action == CommonActions.ACTION_IMAGES_UPDATETASK)
            {
                await Task.Run(() => ImageDatabaseTask.RegisterBackgroundTask());
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEAPI ||
                e.Action == CommonActions.ACTION_WEATHER_UPDATE)
            {
                await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEUNIT)
            {
                await Task.Run(WeatherTileUpdaterTask.RequestAppTrigger);
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEREFRESH || 
                e.Action == CommonActions.ACTION_WEATHER_REREGISTERTASK)
            {
                await Task.Run(() => WeatherUpdateBackgroundTask.RegisterBackgroundTask(true));
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEGPS)
            {
                // Update tile ids when switching GPS feature
                if (Settings.FollowGPS)
                {
                    var prevLoc = (await Settings.GetFavorites()).FirstOrDefault();
                    if (prevLoc?.query != null && SecondaryTileUtils.Exists(prevLoc.query))
                    {
                        var gpsLoc = await Settings.GetLastGPSLocData();
                        if (gpsLoc?.query == null)
                            Settings.SaveLastGPSLocData(prevLoc);
                        else
                            SecondaryTileUtils.UpdateTileId(prevLoc.query, Constants.KEY_GPS);
                    }
                }
                else
                {
                    if (SecondaryTileUtils.Exists(Constants.KEY_GPS))
                    {
                        var favLoc = (await Settings.GetFavorites()).FirstOrDefault();
                        if (favLoc?.IsValid() == true)
                            SecondaryTileUtils.UpdateTileId(Constants.KEY_GPS, favLoc.query);
                    }
                }
            }
            else if (e.Action == CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE)
            {
                object forceUpdate = true;
                if (e.Extras?.TryGetValue(CommonActions.EXTRA_FORCEUPDATE, out forceUpdate) != true || (bool)forceUpdate)
                {
                    await Task.Run(WeatherUpdateBackgroundTask.RequestAppTrigger);
                }
            }
            else if (e.Action == CommonActions.ACTION_SETTINGS_UPDATEDAILYNOTIFICATION)
            {
                if (Settings.DailyNotificationEnabled)
                {
                    Task.Run(() => DailyNotificationTask.RegisterBackgroundTask(false));
                }
                else
                {
                    Task.Run(() => DailyNotificationTask.UnregisterBackgroundTask());
                }
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
