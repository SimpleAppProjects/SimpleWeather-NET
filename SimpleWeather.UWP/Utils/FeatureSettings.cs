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
        public const string KEY_UPDATED = "key_updated";

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
            if (!featureSettings.Values.ContainsKey(KEY_BGIMAGE) || featureSettings.Values[KEY_BGIMAGE] == null)
            {
                SetBackgroundImageEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_BGIMAGE];
        }

        private static void SetBackgroundImageEnabled(bool value)
        {
            featureSettings.Values[KEY_BGIMAGE] = value;
            WasUpdated = true;
        }

        private static bool GetForecastEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_FORECAST) || featureSettings.Values[KEY_FORECAST] == null)
            {
                SetForecastEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_FORECAST];
        }

        private static void SetForecastEnabled(bool value)
        {
            featureSettings.Values[KEY_FORECAST] = value;
            WasUpdated = true;
        }

        private static bool GetHourlyForecastEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_HRFORECAST) || featureSettings.Values[KEY_HRFORECAST] == null)
            {
                SetHourlyForecastEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_HRFORECAST];
        }

        private static void SetHourlyForecastEnabled(bool value)
        {
            featureSettings.Values[KEY_HRFORECAST] = value;
            WasUpdated = true;
        }

        private static bool GetWeatherDetailsEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_WEATHERDETAILS) || featureSettings.Values[KEY_WEATHERDETAILS] == null)
            {
                SetWeatherDetailsEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_WEATHERDETAILS];
        }

        private static void SetWeatherDetailsEnabled(bool value)
        {
            featureSettings.Values[KEY_WEATHERDETAILS] = value;
            WasUpdated = true;
        }

        private static bool GetUVEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_UVINDEX) || featureSettings.Values[KEY_UVINDEX] == null)
            {
                SetUVEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_UVINDEX];
        }

        private static void SetUVEnabled(bool value)
        {
            featureSettings.Values[KEY_UVINDEX] = value;
            WasUpdated = true;
        }

        private static bool GetBeaufortEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_BEAUFORT) || featureSettings.Values[KEY_BEAUFORT] == null)
            {
                SetBeaufortEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_BEAUFORT];
        }

        private static void SetBeaufortEnabled(bool value)
        {
            featureSettings.Values[KEY_BEAUFORT] = value;
            WasUpdated = true;
        }

        private static bool GetAQIEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_AQINDEX) || featureSettings.Values[KEY_AQINDEX] == null)
            {
                SetAQIEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_AQINDEX];
        }

        private static void SetAQIEnabled(bool value)
        {
            featureSettings.Values[KEY_AQINDEX] = value;
            WasUpdated = true;
        }

        private static bool GetMoonPhaseEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_MOONPHASE) || featureSettings.Values[KEY_MOONPHASE] == null)
            {
                SetMoonPhaseEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_MOONPHASE];
        }

        private static void SetMoonPhaseEnabled(bool value)
        {
            featureSettings.Values[KEY_MOONPHASE] = value;
            WasUpdated = true;
        }

        private static bool GetSunPhaseEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_SUNPHASE) || featureSettings.Values[KEY_SUNPHASE] == null)
            {
                SetSunPhaseEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_SUNPHASE];
        }

        private static void SetSunPhaseEnabled(bool value)
        {
            featureSettings.Values[KEY_SUNPHASE] = value;
            WasUpdated = true;
        }

        private static bool GetRadarEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_RADAR) || featureSettings.Values[KEY_RADAR] == null)
            {
                SetRadarEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_RADAR];
        }

        private static void SetRadarEnabled(bool value)
        {
            featureSettings.Values[KEY_RADAR] = value;
            WasUpdated = true;
        }

        private static bool GetPanelBackgroundImageEnabled()
        {
            if (!featureSettings.Values.ContainsKey(KEY_LOCPANELBGIMAGE) || featureSettings.Values[KEY_LOCPANELBGIMAGE] == null)
            {
                SetPanelBackgroundImageEnabled(true);
                return true;
            }
            else
                return (bool)featureSettings.Values[KEY_LOCPANELBGIMAGE];
        }

        private static void SetPanelBackgroundImageEnabled(bool value)
        {
            featureSettings.Values[KEY_LOCPANELBGIMAGE] = value;
            WasUpdated = true;
        }
    }
}