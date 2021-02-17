using SimpleWeather.Utils;
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
            if (e.Action == CommonActions.ACTION_WEATHER_UPDATETILELOCATION)
            {
                if (e.Extras?.ContainsKey(Constants.TILEKEY_OLDKEY) == true)
                {
                    string oldKey = e.Extras?[Constants.TILEKEY_OLDKEY];
                    string locationQuery = e.Extras?[Constants.TILEKEY_LOCATION];

                    if (SecondaryTileUtils.Exists(oldKey))
                    {
                        SecondaryTileUtils.UpdateTileId(oldKey, locationQuery);
                    }
                }
            }
            else if (e.Action == CommonActions.ACTION_IMAGES_UPDATETASK)
            {
                await BackgroundTasks.ImageDatabaseTask.RegisterBackgroundTask();
            }
        }
    }
}
