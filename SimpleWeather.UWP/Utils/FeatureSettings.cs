using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace SimpleWeather.UWP.Utils
{
    public static class FeatureSettings
    {
        public static bool BackgroundImage { get { return GetBackgroundImageEnabled(); } set { SetBackgroundImageEnabled(value); } }
        public static bool Forecast { get { return GetForecastEnabled(); } set { SetForecastEnabled(value); } }
        public static bool HourlyForecast { get { return GetHourlyForecastEnabled(); } set { SetHourlyForecastEnabled(value); } }
        public static bool WeatherDetails { get { return GetWeatherDetailsEnabled(); } set { SetWeatherDetailsEnabled(value); } }
        public static bool UV { get { return GetUVEnabled(); } set { SetUVEnabled(value); } }
        public static bool Beaufort { get { return GetBeaufortEnabled(); } set { SetBeaufortEnabled(value); } }
        public static bool AQIndex { get { return GetAQIEnabled(); } set { SetAQIEnabled(value); } }
        public static bool MoonPhase { get { return GetMoonPhaseEnabled(); } set { SetMoonPhaseEnabled(value); } }
        public static bool SunPhase { get { return GetSunPhaseEnabled(); } set { SetSunPhaseEnabled(value); } }
        public static bool WeatherRadar { get { return GetRadarEnabled(); } set { SetRadarEnabled(value); } }

        public static bool DetailsEnabled => WeatherDetails || ExtraDetailsEnabled;
        public static bool ExtraDetailsEnabled => UV || Beaufort || AQIndex || MoonPhase;

        public static bool LocationPanelBackgroundImage { get { return GetPanelBackgroundImageEnabled(); } set { SetPanelBackgroundImageEnabled(value); } }
        public static bool TileBackgroundImage { get { return GetTileBackgroundImageEnabled(); } set { SetTileBackgroundImageEnabled(value); } }

        public static bool IsUpdateAvailable { get { return GetUpdateAvailable(); } set { SetUpdateAvailable(value); } }

        #region Settings Keys

        private const string KEY_BGIMAGE = "key_bgimage";
        private const string KEY_FORECAST = "key_forecast";
        private const string KEY_HRFORECAST = "key_hrforecast";
        private const string KEY_WEATHERDETAILS = "key_weatherdetails";
        private const string KEY_UVINDEX = "key_uvindex";
        private const string KEY_BEAUFORT = "key_beaufort";
        private const string KEY_AQINDEX = "key_aqindex";
        private const string KEY_MOONPHASE = "key_moonphase";
        private const string KEY_SUNPHASE = "key_sunphase";
        private const string KEY_RADAR = "key_radar";
        private const string KEY_LOCPANELBGIMAGE = "key_locpanelbgimage";
        private const string KEY_TILEBGIMAGE = "key_tilebgimage";
        public const string KEY_UPDATED = "key_updated";

        private const string KEY_UPDATEAVAILABLE = "key_updateavailable";
        #endregion Settings Keys

        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer featureSettings =
            localSettings.CreateContainer("features", ApplicationDataCreateDisposition.Always);

        public static bool WasUpdated
        {
            get
            {
                return CoreApplication.Properties.ContainsKey(KEY_UPDATED) && CoreApplication.Properties[KEY_UPDATED] is bool updated && updated;
            }

            set
            {
                CoreApplication.Properties[KEY_UPDATED] = value;
            }
        }

        private static bool GetBackgroundImageEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_BGIMAGE, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetBackgroundImageEnabled(bool value)
        {
            featureSettings.Values[KEY_BGIMAGE] = value;
            WasUpdated = true;
        }

        private static bool GetForecastEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_FORECAST, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetForecastEnabled(bool value)
        {
            featureSettings.Values[KEY_FORECAST] = value;
            WasUpdated = true;
        }

        private static bool GetHourlyForecastEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_HRFORECAST, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetHourlyForecastEnabled(bool value)
        {
            featureSettings.Values[KEY_HRFORECAST] = value;
            WasUpdated = true;
        }

        private static bool GetWeatherDetailsEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_WEATHERDETAILS, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetWeatherDetailsEnabled(bool value)
        {
            featureSettings.Values[KEY_WEATHERDETAILS] = value;
            WasUpdated = true;
        }

        private static bool GetUVEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_UVINDEX, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetUVEnabled(bool value)
        {
            featureSettings.Values[KEY_UVINDEX] = value;
            WasUpdated = true;
        }

        private static bool GetBeaufortEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_BEAUFORT, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetBeaufortEnabled(bool value)
        {
            featureSettings.Values[KEY_BEAUFORT] = value;
            WasUpdated = true;
        }

        private static bool GetAQIEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_AQINDEX, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetAQIEnabled(bool value)
        {
            featureSettings.Values[KEY_AQINDEX] = value;
            WasUpdated = true;
        }

        private static bool GetMoonPhaseEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_MOONPHASE, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetMoonPhaseEnabled(bool value)
        {
            featureSettings.Values[KEY_MOONPHASE] = value;
            WasUpdated = true;
        }

        private static bool GetSunPhaseEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_SUNPHASE, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetSunPhaseEnabled(bool value)
        {
            featureSettings.Values[KEY_SUNPHASE] = value;
            WasUpdated = true;
        }

        private static bool GetRadarEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_RADAR, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetRadarEnabled(bool value)
        {
            featureSettings.Values[KEY_RADAR] = value;
            WasUpdated = true;
        }

        private static bool GetPanelBackgroundImageEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_LOCPANELBGIMAGE, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetPanelBackgroundImageEnabled(bool value)
        {
            featureSettings.Values[KEY_LOCPANELBGIMAGE] = value;
            WasUpdated = true;
        }

        private static bool GetTileBackgroundImageEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_TILEBGIMAGE, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetTileBackgroundImageEnabled(bool value)
        {
            featureSettings.Values[KEY_TILEBGIMAGE] = value;
            Task.Run(BackgroundTasks.WeatherUpdateBackgroundTask.RequestAppTrigger);
        }

        private static bool GetUpdateAvailable()
        {
            if (featureSettings.Values.TryGetValue(KEY_UPDATEAVAILABLE, out object value))
            {
                return (bool)value;
            }

            return false;
        }

        private static void SetUpdateAvailable(bool value)
        {
            featureSettings.Values[KEY_UPDATEAVAILABLE] = value;
        }
    }
}