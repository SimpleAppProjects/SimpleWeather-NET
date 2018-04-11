#if __ANDROID__
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Wearable;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Droid
{
    public enum WearableDataSync
    {
        Off = 0,
        DeviceOnly,
    }

    public enum WearConnectionStatus
    {
        Disconnected = 0,
        Connecting,
        AppNotInstalled,
        Connected
    }

    public class WearableHelper
    {
        // Name of capability listed in Phone app's wear.xml
        public const String CAPABILITY_PHONE_APP = "com.thewizrd.simpleweather_phone_app";
        // Name of capability listed in Wear app's wear.xml
        public const String CAPABILITY_WEAR_APP = "com.thewizrd.simpleweather_wear_app";

        // Link to Play Store listing
        public const String PLAY_STORE_APP_URI = "market://details?id=com.thewizrd.simpleweather";
        public static Android.Net.Uri PlayStoreURI =
            Android.Net.Uri.Parse(PLAY_STORE_APP_URI);

        // For WearableListenerService
        public const String StartActivityPath = "/start-activity";
        public const String SettingsPath = "/settings";
        public const String LocationPath = "/data/location";
        public const String WeatherPath = "/data/weather";
        public const String ErrorPath = "/error";
        public const String IsSetupPath = "/isweatherloaded";

        public static bool IsGooglePlayServicesInstalled
        {
            get
            {
                var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);
                if (queryResult == ConnectionResult.Success)
                {
                    Log.Info("App", "Google Play Services is installed on this device.");
                    return true;
                }

                if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
                {
                    var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                    Log.Error("App", "There is a problem with Google Play Services on this device: {0} - {1}",
                              queryResult, errorString);
                }

                return false;
            }
        }

        public static bool HasGPS
        {
            get
            {
                return Application.Context.PackageManager.HasSystemFeature(PackageManager.FeatureLocationGps);
            }
        }

        public static Android.Net.Uri GetWearDataUri(String NodeId, String Path)
        {
            return new Android.Net.Uri.Builder()
                .Scheme(PutDataRequest.WearUriScheme)
                .Authority(NodeId)
                .Path(Path)
                .Build();
        }
    }
}
#endif