using System;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.Utils;
using System.Diagnostics;
using System.Globalization;
#if WINDOWS_UWP
using Windows.System.UserProfile;
#endif

namespace SimpleWeather.WeatherData
{
    public class WeatherDataLoader
    {
        private IWeatherLoadedListener callback;
        private IWeatherErrorListener errorCallback;

        private LocationData location = null;
        private Weather weather = null;
        private WeatherManager wm;

        public WeatherDataLoader(IWeatherLoadedListener listener, LocationData location)
        {
            wm = WeatherManager.GetInstance();

            callback = listener;
            this.location = location;
        }

        public WeatherDataLoader(IWeatherLoadedListener listener, IWeatherErrorListener errorListener, LocationData location)
        {
            wm = WeatherManager.GetInstance();

            callback = listener;
            errorCallback = errorListener;
            this.location = location;
        }

        public void SetWeatherLoadedListener(IWeatherLoadedListener listener)
        {
            callback = listener;
        }

        public void SetWeatherErrorListener(IWeatherErrorListener listener)
        {
            errorCallback = listener;
        }

        private async Task GetWeatherData()
        {
            WeatherException wEx = null;
            bool loadedSavedData = false;

            try
            {
                if (wm.NeedsExternalLocationData)
                    weather = await wm.GetWeather(location);
                else
                    weather = await wm.GetWeather(location.query);
            }
            catch (WeatherException weatherEx)
            {
                wEx = weatherEx;
                weather = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                weather = null;
            }

            // Load old data if available and we can't get new data
            if (weather == null)
            {
                loadedSavedData = await LoadSavedWeatherData(true);
            }
            else if (weather != null)
            {
                // Handle upgrades
                if (String.IsNullOrEmpty(location.name) || String.IsNullOrEmpty(location.tz_short))
                {
                    location.name = weather.location.name;
                    location.tz_offset = weather.location.tz_offset;
                    location.tz_short = weather.location.tz_short;

                    await Settings.UpdateLocation(location);
                }

                await SaveWeatherData();
            }

            // Throw exception if we're unable to get any weather data
            if (weather == null && wEx != null)
            {
                throw wEx;
            }
            else if (weather == null && wEx == null)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null && wEx != null && loadedSavedData)
            {
                throw wEx;
            }
        }

        public async Task LoadWeatherData(bool forceRefresh)
        {
            if (forceRefresh)
            {
                try
                {
                    await GetWeatherData();
                }
                catch (WeatherException wEx)
                {
                    errorCallback?.OnWeatherError(wEx);
                }
            }
            else
                await LoadWeatherData();

            callback?.OnWeatherLoaded(location, weather);
        }

        private async Task LoadWeatherData()
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            bool gotData = await LoadSavedWeatherData();

            if (!gotData)
            {
                try
                {
                    if (weather != null && weather.source != Settings.API)
                        await wm.UpdateLocationQuery(weather);

                    await GetWeatherData();
                }
                catch (WeatherException wEx)
                {
                    errorCallback?.OnWeatherError(wEx);
                }
            }
        }

        private async Task<bool> LoadSavedWeatherData(bool _override)
        {
            if (_override)
            {
                // Load weather data
                try
                {
                    weather = await Settings.GetWeatherData(location.query);
                }
                catch (Exception ex)
                {
                    weather = null;
                    Debug.WriteLine(ex.StackTrace);
                }

#if WINDOWS_UWP
                var userlang = GlobalizationPreferences.Languages.First();
                var culture = new CultureInfo(userlang);
#else
                var culture = CultureInfo.CurrentCulture;
#endif
                var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                bool isValid = weather == null || !weather.IsValid() || weather.source != Settings.API;
                if (wm.SupportsWeatherLocale && !isValid)
                    isValid = weather.locale != locale;

                if (isValid) return false;

                return true;
            }
            else
                return await LoadSavedWeatherData();
        }

        private async Task<bool> LoadSavedWeatherData()
        {
            // Load weather data
            try
            {
                weather = await Settings.GetWeatherData(location.query);
            }
            catch (Exception ex)
            {
                weather = null;
                Debug.WriteLine(ex.StackTrace);
            }

#if WINDOWS_UWP
            var userlang = GlobalizationPreferences.Languages.First();
            var culture = new CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif
            var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            bool isValid = weather == null || !weather.IsValid() || weather.source != Settings.API;
            if (wm.SupportsWeatherLocale && !isValid)
                isValid = weather.locale != locale;

            if (isValid) return false;

            // Weather data expiration
            if (!int.TryParse(weather.ttl, out int ttl))
                ttl = 60;

            // Check file age
            DateTimeOffset updateTime = weather.update_time;

            TimeSpan span = DateTimeOffset.Now - updateTime;
            if (span.TotalMinutes < ttl)
                return true;
            else
                return false;
        }

        private async Task SaveWeatherData()
        {
            // Save location query
            weather.query = location.query;

            await Settings.SaveWeatherData(weather);
        }

        public Weather GetWeather()
        {
            return weather;
        }
    }
}
