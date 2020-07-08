using SimpleWeather.Location;
using SimpleWeather.Utils;
#if WINDOWS_UWP && !UNIT_TEST
using SimpleWeather.UWP.Tiles;
#endif
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace SimpleWeather.WeatherData
{
    public class WeatherDataLoader
    {
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

        /// <summary>
        /// GetWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        private ConfiguredTaskAwaitable GetWeatherData(WeatherRequest request)
        {
            return AsyncTask.CreateTask(async () =>
            {
                WeatherException wEx = null;
                bool loadedSavedData = false;

                try
                {
                    if (WeatherAPI.NWS.Equals(Settings.API) && !"US".Equals(location.country_code))
                    {
                        // If location data hasn't been updated, try loading weather from the previous provider
                        if (!String.IsNullOrWhiteSpace(location.weatherSource) &&
                            !WeatherAPI.NWS.Equals(location.weatherSource))
                        {
                            weather = await WeatherManager.GetProvider(location.weatherSource).GetWeather(location);
                        }
                        else
                        {
                            throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                        }
                    }
                    else
                    {
                        // Load weather from provider
                        weather = await wm.GetWeather(location);
                    }
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
                    loadedSavedData = await LoadSavedWeatherData(request, true);
                }
                else if (weather != null)
                {
                    // Handle upgrades
                    if (String.IsNullOrEmpty(location.name) || String.IsNullOrEmpty(location.tz_long))
                    {
                        location.name = weather.location.name;
                        location.tz_long = weather.location.tz_long;

                        await Settings.UpdateLocation(location);
                    }
                    if (location.latitude == 0 && location.longitude == 0 &&
                        weather.location.latitude.HasValue && weather.location.longitude.HasValue)
                    {
                        location.latitude = weather.location.latitude.Value;
                        location.longitude = weather.location.longitude.Value;

                        await Settings.UpdateLocation(location);
                    }
                    if (String.IsNullOrWhiteSpace(location.locationSource))
                    {
                        location.locationSource = wm.LocationProvider.LocationAPI;
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
            });
        }

        /// <summary>
        /// LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        public Task<Weather> LoadWeatherData(WeatherRequest request)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (request.ForceLoadSavedData)
                    {
                        await LoadSavedWeatherData(request, true);
                    }
                    else
                    {
                        if (request.ForceRefresh)
                        {
                            await GetWeatherData(request);
                        }
                        else
                        {
                            if (!IsDataValid(false))
                            {
                                await _LoadWeatherData(request);
                            }
                        }
                    }
                    await CheckForOutdatedObservation();
                }
                catch (WeatherException wEx)
                {
                    if (request.ErrorListener != null)
                        request.ErrorListener.OnWeatherError(wEx);
                    else
                        throw wEx;
                }

                Logger.WriteLine(LoggerLevel.Debug, "{0}: Sending weather data to callback", TAG);
                Logger.WriteLine(LoggerLevel.Debug, "{0}: Weather data for {1} is valid = {2}", TAG, location?.ToString(), weather?.IsValid());

                return weather;
            });
        }

        /// <summary>
        /// _LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        private ConfiguredTaskAwaitable _LoadWeatherData(WeatherRequest request)
        {
            return AsyncTask.CreateTask(async () =>
            {
                /*
                 * If unable to retrieve saved data, data is old, or units don't match
                 * Refresh weather data
                */

                Logger.WriteLine(LoggerLevel.Debug, "{0}: Loading weather data for {1}", TAG, location?.ToString());

                bool gotData = await LoadSavedWeatherData(request);

                if (!gotData)
                {
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Saved weather data invalid for {1}", TAG, location?.ToString());
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Retrieving data from weather provider", TAG);

                    if ((weather != null && weather.source != Settings.API)
                        || (weather == null && location != null && location.weatherSource != Settings.API))
                    {
                        if (!WeatherAPI.NWS.Equals(location.weatherSource) || "US".Equals(location.country_code))
                        {
                            // Update location query and source for new API
                            string oldKey = location.query;

                            if (weather != null)
                                location.query = wm.UpdateLocationQuery(weather);
                            else
                                location.query = wm.UpdateLocationQuery(location);

                            location.weatherSource = Settings.API;

                            // Update database as well
                            if (location.locationType == LocationType.GPS)
                                Settings.SaveLastGPSLocData(location);
                            else
                                await Settings.UpdateLocationWithKey(location, oldKey);

#if WINDOWS_UWP && !UNIT_TEST
                            // Update tile id for location
                            if (SecondaryTileUtils.Exists(oldKey))
                            {
                                await SecondaryTileUtils.UpdateTileId(oldKey, location.query);
                            }
#endif
                        }
                    }

                    await GetWeatherData(request);
                }
            });
        }

        private ConfiguredTaskAwaitable<bool> LoadSavedWeatherData(WeatherRequest request)
        {
            return LoadSavedWeatherData(request, false);
        }

        private ConfiguredTaskAwaitable<bool> LoadSavedWeatherData(WeatherRequest request, bool _override)
        {
            return AsyncTask.CreateTask(async () =>
            {
                // Load weather data
                try
                {
                    weather = await Settings.GetWeatherData(location.query);

                    if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                        weather.weather_alerts = await Settings.GetWeatherAlertData(location.query);

                    if (request.LoadForecasts && weather != null)
                    {
                        var forecasts = await Settings.GetWeatherForecastData(location.query);
                        var hrForecasts = await Settings.GetHourlyWeatherForecastData(location.query);
                        weather.forecast = forecasts?.forecast;
                        weather.hr_forecast = hrForecasts;
                        weather.txt_forecast = forecasts?.txt_forecast;
                    }

                    if (_override && weather == null)
                    {
                        // If weather is still unavailable try manually searching for it
                        weather = await Settings.GetWeatherDataByCoordinate(location);

                        if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                            weather.weather_alerts = await Settings.GetWeatherAlertData(weather.query);

                        if (request.LoadForecasts && weather != null)
                        {
                            var forecasts = await Settings.GetWeatherForecastData(location.query);
                            var hrForecasts = await Settings.GetHourlyWeatherForecastData(location.query);
                            weather.forecast = forecasts?.forecast;
                            weather.hr_forecast = hrForecasts;
                            weather.txt_forecast = forecasts?.txt_forecast;
                        }
                    }
                }
                catch (Exception ex)
                {
                    weather = null;
                    Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error loading saved weather data");
                }

                return IsDataValid(_override);
            });
        }

        private ConfiguredTaskAwaitable CheckForOutdatedObservation()
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (weather != null)
                {
                    // Check for outdated observation
                    var now = DateTimeOffset.Now.ToOffset(location.tz_offset);
                    var durationMins = (now - weather.condition.observation_time).TotalMinutes;
                    if (durationMins > 60)
                    {
                        var hrf = await Settings.GetFirstHourlyWeatherForecastDataByDate(location.query, now.Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz"));

                        if (hrf != null)
                        {
                            weather.condition.weather = hrf.condition;
                            weather.condition.icon = hrf.icon;

                            weather.condition.temp_f = hrf.high_f;
                            weather.condition.temp_c = hrf.high_c;

                            weather.condition.wind_mph = hrf.wind_mph;
                            weather.condition.wind_kph = hrf.wind_kph;
                            weather.condition.wind_degrees = hrf.wind_degrees;

                            if (hrf.wind_mph.HasValue)
                            {
                                weather.condition.beaufort = new Beaufort((int)WeatherUtils.GetBeaufortScale((int)Math.Round(hrf.wind_mph.Value)));
                            }
                            weather.condition.feelslike_f = hrf.extras?.feelslike_f ?? 0.0f;
                            weather.condition.feelslike_c = hrf.extras?.feelslike_c ?? 0.0f;
                            weather.condition.uv = hrf.extras?.uv_index.HasValue == true && hrf.extras?.uv_index >= 0 ? new UV(hrf.extras.uv_index.Value) : null;

                            weather.condition.observation_time = hrf.date;

                            if (durationMins > 60 * 6)
                            {
                                var fcasts = await Settings.GetWeatherForecastData(location.query);
                                var fcast = fcasts.forecast?.FirstOrDefault(f => f.date.Date == now.Date);

                                if (fcast != null)
                                {
                                    weather.condition.high_f = fcast.high_f;
                                    weather.condition.high_c = fcast.high_c;
                                    weather.condition.low_f = fcast.low_f;
                                    weather.condition.low_c = fcast.low_c;
                                }
                                else
                                {
                                    weather.condition.high_f = weather.condition.high_c = 0f;
                                    weather.condition.low_f = weather.condition.low_c = 0f;
                                }
                            }

                            weather.atmosphere.dewpoint_f = hrf.extras?.dewpoint_f;
                            weather.atmosphere.dewpoint_c = hrf.extras?.dewpoint_c;
                            weather.atmosphere.humidity = hrf.extras?.humidity;
                            weather.atmosphere.pressure_trend = null;
                            weather.atmosphere.pressure_in = hrf.extras?.pressure_in;
                            weather.atmosphere.pressure_mb = hrf.extras?.pressure_mb;
                            weather.atmosphere.visibility_mi = hrf.extras?.visibility_mi;
                            weather.atmosphere.visibility_km = hrf.extras?.visibility_km;

                            if (weather.precipitation != null)
                            {
                                weather.precipitation.pop = hrf.extras?.pop;
                                weather.precipitation.qpf_rain_in = hrf.extras?.qpf_rain_in >= 0 ? hrf.extras.qpf_rain_in : 0.0f;
                                weather.precipitation.qpf_rain_mm = hrf.extras?.qpf_rain_mm >= 0 ? hrf.extras.qpf_rain_mm : 0.0f;
                                weather.precipitation.qpf_snow_in = hrf.extras?.qpf_snow_in >= 0 ? hrf.extras.qpf_snow_in : 0.0f;
                                weather.precipitation.qpf_snow_cm = hrf.extras?.qpf_snow_cm >= 0 ? hrf.extras.qpf_snow_cm : 0.0f;
                            }

                            await Settings.SaveWeatherData(weather);
                        }
                    }
                }
            });
        }

        private bool IsDataValid(bool _override)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);
            var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            String API = Settings.API;
            bool isInvalid = weather == null || !weather.IsValid();
            if (!isInvalid && !String.Equals(weather.source, API))
            {
                if (!WeatherAPI.NWS.Equals(API) || "US".Equals(location.country_code))
                    isInvalid = true;
            }

            if (wm.SupportsWeatherLocale && !isInvalid)
                isInvalid = !String.Equals(weather.locale, locale);

            if (_override || isInvalid) return !isInvalid;

            // Weather data expiration
            int ttl = Math.Max(weather.ttl, Settings.RefreshInterval);

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

            await SaveWeatherForecasts();

            await Settings.SaveWeatherData(weather);
        }

        private ConfiguredTaskAwaitable SaveWeatherAlerts()
        {
            return AsyncTask.CreateTask(async () =>
            {
                if (weather.weather_alerts != null)
                {
                    // Check for previously saved alerts
                    var previousAlerts = await Settings.GetWeatherAlertData(location.query);

                    if (previousAlerts != null && previousAlerts.Any())
                    {
                        // If any previous alerts were flagged before as notified
                        // make sure to set them here as such
                        // bc notified flag gets reset when retrieving weatherdata
                        foreach (WeatherAlert alert in weather.weather_alerts)
                        {
                            if (previousAlerts.FirstOrDefault(walert => walert.Equals(alert)) is WeatherAlert prevAlert)
                            {
                                if (prevAlert.Notified)
                                    alert.Notified = prevAlert.Notified;
                            }
                        }
                    }

                    await Settings.SaveWeatherAlerts(location, weather.weather_alerts);
                }
            });
        }

        private async Task SaveWeatherForecasts()
        {
            await Settings.SaveWeatherForecasts(new Forecasts()
            {
                query = weather.query,
                forecast = weather.forecast,
                txt_forecast = weather.txt_forecast
            });
            await Settings.SaveWeatherForecasts(location, weather?.hr_forecast?.Select(f => new HourlyForecasts(weather?.query, f)));
        }
    }
}