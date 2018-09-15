using System;
using System.Linq;
using System.Threading.Tasks;
using SimpleWeather.Utils;
using System.Globalization;
#if WINDOWS_UWP
using SimpleWeather.UWP;
using SimpleWeather.UWP.Helpers;
using Windows.System.UserProfile;
#elif __ANDROID__
using Android.App;
using SimpleWeather.Droid;
#if !__ANDROID_WEAR__
using SimpleWeather.Droid.App;
using SimpleWeather.Droid.App.Widgets;
#endif
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

        private const String TAG = nameof(WeatherDataLoader);

        public WeatherDataLoader(LocationData location)
        {
            wm = WeatherManager.GetInstance();

            this.location = location;

            if (this.location == null)
                throw new ArgumentNullException(nameof(location));
        }

        public WeatherDataLoader(LocationData location, IWeatherLoadedListener listener)
            : this(location)
        {
            callback = listener;
        }

        public WeatherDataLoader(LocationData location, IWeatherLoadedListener listener, IWeatherErrorListener errorListener)
            : this(location, listener)
        {
            errorCallback = errorListener;
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
                weather = await wm.GetWeather(location);
            }
            catch (WeatherException weatherEx)
            {
                wEx = weatherEx;
                weather = null;
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error getting weather data");
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
                if (String.IsNullOrEmpty(location.name) || String.IsNullOrEmpty(location.tz_long))
                {
                    location.name = weather.location.name;
                    location.tz_long = weather.location.tz_long;

#if !__ANDROID_WEAR__
                    await Settings.UpdateLocation(location);
#else
                    Settings.SaveHomeData(location);
#endif
                }
                if (location.latitude == 0 && location.longitude == 0)
                {
                    location.latitude = double.Parse(weather.location.latitude);
                    location.longitude = double.Parse(weather.location.longitude);
#if !__ANDROID_WEAR__
                    await Settings.UpdateLocation(location);
#else
                    Settings.SaveHomeData(location);
#endif
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

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Sending weather data to callback", TAG);
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Weather data for {1} is valid = {2}", TAG, location?.ToString(), weather?.IsValid());

            callback?.OnWeatherLoaded(location, weather);
        }

        private async Task LoadWeatherData()
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
            */

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Loading weather data for {1}", TAG, location?.ToString());

            bool gotData = await LoadSavedWeatherData();

            if (!gotData)
            {
                Logger.WriteLine(LoggerLevel.Debug, "{0}: Saved weather data invalid for {1}", TAG, location?.ToString());
                Logger.WriteLine(LoggerLevel.Debug, "{0}: Retrieving data from weather provider", TAG);

                try
                {
                    if ((weather != null && weather.source != Settings.API)
                        || (weather == null && location != null && location.source != Settings.API))
                    {
                        // Update location query and source for new API
                        string oldKey = location.query;

                        if (weather != null)
                            location.query = await wm.UpdateLocationQuery(weather);
                        else
                            location.query = await wm.UpdateLocationQuery(location);

                        location.source = Settings.API;

                        // Update database as well
#if !__ANDROID_WEAR__
                        if (location.locationType == LocationType.GPS)
                        {
                            Settings.SaveLastGPSLocData(location);
#if __ANDROID__
                            WearableDataListenerService.EnqueueWork(App.Context,
                                new Android.Content.Intent(App.Context, typeof(WearableDataListenerService))
                                    .SetAction(WearableDataListenerService.ACTION_SENDLOCATIONUPDATE));
#endif
                        }
                        else
                            await Settings.UpdateLocationWithKey(location, oldKey);
#else
                        Settings.SaveHomeData(location);
#endif

#if WINDOWS_UWP
                        // Update tile id for location
                        if (SecondaryTileUtils.Exists(oldKey))
                        {
                            await SecondaryTileUtils.UpdateTileId(oldKey, location.query);
                        }
#elif __ANDROID__ && !__ANDROID_WEAR__
                        if (WidgetUtils.Exists(oldKey))
                        {
                            WidgetUtils.UpdateWidgetIds(oldKey, location);
                        }
#endif
                    }

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

                    if (wm.SupportsAlerts)
                        weather.weather_alerts = await Settings.GetWeatherAlertData(location.query);
                }
                catch (Exception ex)
                {
                    weather = null;
                    Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error loading saved weather data");
                }

#if WINDOWS_UWP
                var userlang = GlobalizationPreferences.Languages.First();
                var culture = new CultureInfo(userlang);
#else
                var culture = CultureInfo.CurrentCulture;
#endif
                var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

                bool isInvalid = weather == null || !weather.IsValid() || weather.source != Settings.API;
                if (wm.SupportsWeatherLocale && !isInvalid)
                    isInvalid = weather.locale != locale;

                return !isInvalid;
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

                if (wm.SupportsAlerts)
                    weather.weather_alerts = await Settings.GetWeatherAlertData(location.query);
            }
            catch (Exception ex)
            {
                weather = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error loading saved weather data");
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
                ttl = Settings.DefaultInterval;

            // Check file age
            DateTimeOffset updateTime = weather.update_time;

            TimeSpan span = DateTimeOffset.Now - updateTime;
            return span.TotalMinutes < ttl;
        }

        private async Task SaveWeatherData()
        {
            // Save location query
            weather.query = location.query;

            // Save weather alerts
            await SaveWeatherAlerts();

            await Settings.SaveWeatherData(weather);

#if !__ANDROID_WEAR__ && __ANDROID__
            // Update weather data for Wearables
            WearableDataListenerService.EnqueueWork(Application.Context,
                new Android.Content.Intent(Application.Context, typeof(WearableDataListenerService))
                    .SetAction(WearableDataListenerService.ACTION_SENDWEATHERUPDATE));

            // Update cached weather data for widgets
            await Task.Run(() =>
            {
                if (WidgetUtils.Exists(location.query))
                {
                    var ids = WidgetUtils.GetWidgetIds(location.query);
                    foreach (int id in ids)
                    {
                        WidgetUtils.SaveWeatherData(id, weather);
                    }
                }
            });
#elif __ANDROID_WEAR__
            Settings.UpdateTime = weather.update_time.UtcDateTime;
#endif
        }

        private async Task SaveWeatherAlerts()
        {
            if (weather.weather_alerts != null)
            {
                // Check for previously saved alerts
                var previousAlerts = await Settings.GetWeatherAlertData(location.query);

                if (previousAlerts != null && previousAlerts.Count > 0)
                {
                    // If any previous alerts were flagged before as notified
                    // make sure to set them here as such
                    // bc notified flag gets reset when retrieving weatherdata
                    foreach(WeatherAlert alert in weather.weather_alerts)
                    {
                        if (previousAlerts.Find(walert => walert.Equals(alert)) is WeatherAlert prevAlert)
                        {
                            if (prevAlert.Notified)
                                alert.Notified = prevAlert.Notified;
                        }
                    }
                }


                await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
            }
        }

        public async Task ForceLoadSavedWeatherData()
        {
            await LoadSavedWeatherData(true);

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Sending weather data to callback", TAG);
            Logger.WriteLine(LoggerLevel.Debug, "{0}: Weather data for {1} is valid = {2}", TAG, location?.ToString(), weather?.IsValid());

            if (weather != null)
                callback?.OnWeatherLoaded(location, weather);
            else
                errorCallback?.OnWeatherError(new WeatherException(WeatherUtils.ErrorStatus.NoWeather));
        }

        public Weather GetWeather()
        {
            return weather;
        }
    }
}
