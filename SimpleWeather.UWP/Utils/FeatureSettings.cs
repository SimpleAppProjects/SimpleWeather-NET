using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using static SimpleWeather.UWP.Utils.FeatureSettingsChangedEventArgs;

namespace SimpleWeather.UWP.Utils
{
    public static class FeatureSettings
    {
        public static bool BackgroundImage { get { return GetBackgroundImageEnabled(); } set { SetBackgroundImageEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_BGIMAGE, NewValue = value }); } }
        public static bool Forecast { get { return GetForecastEnabled(); } set { SetForecastEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_FORECAST, NewValue = value }); } }
        public static bool HourlyForecast { get { return GetHourlyForecastEnabled(); } set { SetHourlyForecastEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_HRFORECAST, NewValue = value }); } }
        public static bool Charts { get { return GetChartsEnabled(); } set { SetChartsEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_CHARTS, NewValue = value }); } }
        public static bool WeatherSummary { get { return GetWeatherSummaryEnabled(); } set { SetWeatherSummaryEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_WEATHERSUMMARY, NewValue = value }); } }
        public static bool WeatherDetails { get { return GetWeatherDetailsEnabled(); } set { SetWeatherDetailsEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_WEATHERDETAILS, NewValue = value }); } }
        public static bool UV { get { return GetUVEnabled(); } set { SetUVEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_UVINDEX, NewValue = value }); } }
        public static bool Beaufort { get { return GetBeaufortEnabled(); } set { SetBeaufortEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_BEAUFORT, NewValue = value }); } }
        public static bool AQIndex { get { return GetAQIEnabled(); } set { SetAQIEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_AQINDEX, NewValue = value }); } }
        public static bool MoonPhase { get { return GetMoonPhaseEnabled(); } set { SetMoonPhaseEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_MOONPHASE, NewValue = value }); } }
        public static bool SunPhase { get { return GetSunPhaseEnabled(); } set { SetSunPhaseEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_SUNPHASE, NewValue = value }); } }
        public static bool WeatherRadar { get { return GetRadarEnabled(); } set { SetRadarEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_RADAR, NewValue = value }); } }

        public static bool DetailsEnabled => WeatherDetails || ExtraDetailsEnabled;
        public static bool ExtraDetailsEnabled => UV || Beaufort || AQIndex || MoonPhase;

        public static bool LocationPanelBackgroundImage { get { return GetPanelBackgroundImageEnabled(); } set { SetPanelBackgroundImageEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_LOCPANELBGIMAGE, NewValue = value }); } }
        public static bool TileBackgroundImage { get { return GetTileBackgroundImageEnabled(); } set { SetTileBackgroundImageEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_TILEBGIMAGE, NewValue = value }); } }

        #region Settings Keys

        private const string KEY_BGIMAGE = "key_bgimage";
        private const string KEY_FORECAST = "key_forecast";
        private const string KEY_HRFORECAST = "key_hrforecast";
        private const string KEY_CHARTS = "key_charts";
        private const string KEY_WEATHERSUMMARY = "key_weathersummary";
        private const string KEY_WEATHERDETAILS = "key_weatherdetails";
        private const string KEY_UVINDEX = "key_uvindex";
        private const string KEY_BEAUFORT = "key_beaufort";
        private const string KEY_AQINDEX = "key_aqindex";
        private const string KEY_MOONPHASE = "key_moonphase";
        private const string KEY_SUNPHASE = "key_sunphase";
        private const string KEY_RADAR = "key_radar";
        private const string KEY_LOCPANELBGIMAGE = "key_locpanelbgimage";
        private const string KEY_TILEBGIMAGE = "key_tilebgimage";

        #endregion Settings Keys

        public static event FeatureSettingsChangedEventHandler OnFeatureSettingsChanged;

        // Shared Settings
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static readonly ApplicationDataContainer featureSettings =
            localSettings.CreateContainer("features", ApplicationDataCreateDisposition.Always);

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
        }

        private static bool GetChartsEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_CHARTS, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetChartsEnabled(bool value)
        {
            featureSettings.Values[KEY_CHARTS] = value;
        }

        private static bool GetWeatherSummaryEnabled()
        {
            if (featureSettings.Values.TryGetValue(KEY_WEATHERSUMMARY, out object value))
            {
                return (bool)value;
            }

            return true;
        }

        private static void SetWeatherSummaryEnabled(bool value)
        {
            featureSettings.Values[KEY_WEATHERSUMMARY] = value;
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
    }
}