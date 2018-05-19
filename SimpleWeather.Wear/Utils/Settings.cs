using Android.App;
using Android.Content;
using Android.Util;
using Java.IO;
using SimpleWeather.Droid;
using SimpleWeather.Droid.Wear;
using SimpleWeather.Droid.Wear.Helpers;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class Settings
    {
        private const string KEY_DATASYNC = "key_datasync";

        public static WearableDataSync DataSync { get { return GetDataSync(); } set { SetDataSync(value); } }

        // Android specific settings
        private static LocationData GetHomeData()
        {
            LocationData homeData = null;

            homeData = Task.Run(() => GetLastGPSLocData()).Result;

            if (homeData != null && !FollowGPS)
                homeData.locationType = LocationType.Search;

            return homeData;
        }

        public static void SaveHomeData(LocationData data)
        {
            lastGPSLocData = data;
            LastGPSLocation = lastGPSLocData?.ToJson();
        }

        private static WearableDataSync GetDataSync()
        {
            return (WearableDataSync)int.Parse(preferences.GetString(KEY_DATASYNC, "0"));
        }

        private static void SetDataSync(WearableDataSync value)
        {
            editor.PutString(KEY_DATASYNC, ((int)value).ToString());
            editor.Commit();
        }
    }
}