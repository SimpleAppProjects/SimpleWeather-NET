using SimpleWeather.Preferences;
using static SimpleWeather.Maui.Utils.FeatureSettingsChangedEventArgs;

namespace SimpleWeather.Maui.Utils
{
    public static class FeatureSettings
    {
        #region Settings Properties
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
        public static bool PollenEnabled { get { return GetPollenEnabled(); } set { SetPollenEnabled(value); OnFeatureSettingsChanged?.Invoke(new FeatureSettingsChangedEventArgs { Key = KEY_POLLEN, NewValue = value }); } }
        #endregion

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
        private const string KEY_POLLEN = "key_pollen";
        #endregion Settings Keys

        public static event FeatureSettingsChangedEventHandler OnFeatureSettingsChanged;

        // Shared Settings
        private static readonly SettingsContainer featureSettings = new SettingsContainer("features");

        private static bool GetBackgroundImageEnabled()
        {
            return featureSettings.GetValue(KEY_BGIMAGE, true);
        }

        private static void SetBackgroundImageEnabled(bool value)
        {
            featureSettings.SetValue(KEY_BGIMAGE, value);
        }

        private static bool GetForecastEnabled()
        {
            return featureSettings.GetValue(KEY_FORECAST, true);
        }

        private static void SetForecastEnabled(bool value)
        {
            featureSettings.SetValue(KEY_FORECAST, value);
        }

        private static bool GetHourlyForecastEnabled()
        {
            return featureSettings.GetValue(KEY_HRFORECAST, true);
        }

        private static void SetHourlyForecastEnabled(bool value)
        {
            featureSettings.SetValue(KEY_HRFORECAST, value);
        }

        private static bool GetChartsEnabled()
        {
            return featureSettings.GetValue(KEY_CHARTS, true);
        }

        private static void SetChartsEnabled(bool value)
        {
            featureSettings.SetValue(KEY_CHARTS, value);
        }

        private static bool GetWeatherSummaryEnabled()
        {
            return featureSettings.GetValue(KEY_WEATHERSUMMARY, true);
        }

        private static void SetWeatherSummaryEnabled(bool value)
        {
            featureSettings.SetValue(KEY_WEATHERSUMMARY, value);
        }

        private static bool GetWeatherDetailsEnabled()
        {
            return featureSettings.GetValue(KEY_WEATHERDETAILS, true);
        }

        private static void SetWeatherDetailsEnabled(bool value)
        {
            featureSettings.SetValue(KEY_WEATHERDETAILS, value);
        }

        private static bool GetUVEnabled()
        {
            return featureSettings.GetValue(KEY_UVINDEX, true);
        }

        private static void SetUVEnabled(bool value)
        {
            featureSettings.SetValue(KEY_UVINDEX, value);
        }

        private static bool GetBeaufortEnabled()
        {
            return featureSettings.GetValue(KEY_BEAUFORT, true);
        }

        private static void SetBeaufortEnabled(bool value)
        {
            featureSettings.SetValue(KEY_BEAUFORT, value);
        }

        private static bool GetAQIEnabled()
        {
            return featureSettings.GetValue(KEY_AQINDEX, true);
        }

        private static void SetAQIEnabled(bool value)
        {
            featureSettings.SetValue(KEY_AQINDEX, value);
        }

        private static bool GetMoonPhaseEnabled()
        {
            return featureSettings.GetValue(KEY_MOONPHASE, true);
        }

        private static void SetMoonPhaseEnabled(bool value)
        {
            featureSettings.SetValue(KEY_MOONPHASE, value);
        }

        private static bool GetSunPhaseEnabled()
        {
            return featureSettings.GetValue(KEY_SUNPHASE, true);
        }

        private static void SetSunPhaseEnabled(bool value)
        {
            featureSettings.SetValue(KEY_SUNPHASE, value);
        }

        private static bool GetRadarEnabled()
        {
            return featureSettings.GetValue(KEY_RADAR, true);
        }

        private static void SetRadarEnabled(bool value)
        {
            featureSettings.SetValue(KEY_RADAR, value);
        }

        private static bool GetPanelBackgroundImageEnabled()
        {
            return featureSettings.GetValue(KEY_LOCPANELBGIMAGE, true);
        }

        private static void SetPanelBackgroundImageEnabled(bool value)
        {
            featureSettings.SetValue(KEY_LOCPANELBGIMAGE, value);
        }

        private static bool GetTileBackgroundImageEnabled()
        {
            return featureSettings.GetValue(KEY_TILEBGIMAGE, true);
        }

        private static void SetTileBackgroundImageEnabled(bool value)
        {
            featureSettings.SetValue(KEY_TILEBGIMAGE, value);
#if WINDOWS_UWP
            Task.Run(BackgroundTasks.WeatherUpdateBackgroundTask.RequestAppTrigger);
#endif
        }

        private static bool GetPollenEnabled()
        {
            return featureSettings.GetValue(KEY_POLLEN, true);
        }

        private static void SetPollenEnabled(bool value)
        {
            featureSettings.SetValue(KEY_POLLEN, value);
        }
    }
}
