using SimpleWeather.Location;
using SimpleWeather.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public sealed class WeatherDataLoader
    {
        private readonly LocationData location;
        private Weather weather = null;
        private ICollection<WeatherAlert> weatherAlerts = null;
        private WeatherManager wm = WeatherManager.GetInstance();

        private const String TAG = nameof(WeatherDataLoader);

        public WeatherDataLoader(LocationData location)
        {
            this.location = location ?? throw new ArgumentNullException(nameof(location));
        }

        /// <summary>
        /// LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        public async Task<Weather> LoadWeatherData(WeatherRequest request)
        {
            return (await LoadWeatherTask(request)).Weather;
        }

        /// <summary>
        /// LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<WeatherResult> LoadWeatherResult(WeatherRequest request)
        {
            return LoadWeatherTask(request);
        }

        /// <exception cref="WeatherException"></exception>
        private Func<WeatherRequest, Task<WeatherResult>> LoadWeatherTask => async (request) =>
        {
            WeatherResult result = null;

            try
            {
                if (request.ForceLoadSavedData)
                {
                    await LoadSavedWeatherData(request, true).ConfigureAwait(false);
                }
                else
                {
                    if (request.ForceRefresh)
                    {
                        result = await GetWeatherData(request).ConfigureAwait(false);
                    }
                    else
                    {
                        if (!IsDataValid(false))
                        {
                            result = await _LoadWeatherData(request).ConfigureAwait(false);
                        }
                    }
                }
                if (request.ShouldSaveData)
                {
                    await CheckForOutdatedObservation().ConfigureAwait(false);
                }
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

            if (result == null)
            {
                result = WeatherResult.Create(weather, false);
            }

            return result;
        };

        public async Task<ICollection<WeatherAlert>> LoadWeatherAlerts(bool loadSavedData)
        {
            if (wm.SupportsAlerts)
            {
                if (wm.NeedsExternalAlertData)
                {
                    if (!loadSavedData)
                    {
                        weatherAlerts = await wm.GetAlerts(location).ConfigureAwait(false);
                    }
                }

                if (weatherAlerts == null)
                {
                    weatherAlerts = await Settings.GetWeatherAlertData(location.query).ConfigureAwait(false);
                }

                if (!loadSavedData)
                {
                    await SaveWeatherAlerts().ConfigureAwait(false);
                }
            }

            return weatherAlerts;
        }

        /// <summary>
        /// GetWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        private async Task<WeatherResult> GetWeatherData(WeatherRequest request)
        {
            WeatherException wEx = null;
            bool loadedSavedData = false;
            bool loadedSavedAlertData = false;

            // Try to get weather from provider API
            try
            {
                request.ThrowIfCancellationRequested();
                weather = null;

                if (!wm.IsRegionSupported(location.country_code))
                {
                    // If location data hasn't been updated, try loading weather from the previous provider
                    if (!String.IsNullOrWhiteSpace(location.weatherSource))
                    {
                        var provider = WeatherManager.GetProvider(location.weatherSource);
                        if (provider.IsRegionSupported(location.country_code))
                        {
                            weather = await provider.GetWeather(location).ConfigureAwait(false);
                        }
                    }

                    // Nothing to fallback on; error out
                    if (weather == null)
                    {
                        throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
                    }
                }
                else
                {
                    // Load weather from provider
                    weather = await wm.GetWeather(location).ConfigureAwait(false);
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

            if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
            {
                if (wm.NeedsExternalAlertData)
                {
                    weather.weather_alerts = await wm.GetAlerts(location).ConfigureAwait(false);
                }

                if (weather.weather_alerts == null)
                {
                    weatherAlerts = weather.weather_alerts = await Settings.GetWeatherAlertData(location.query).ConfigureAwait(false);
                    loadedSavedAlertData = true;
                }
            }

            if (request.ShouldSaveData)
            {
                // Load old data if available and we can't get new data
                if (weather == null)
                {
                    loadedSavedData = await LoadSavedWeatherData(request, true).ConfigureAwait(false);
                    loadedSavedAlertData = loadedSavedData;
                }
                else if (weather != null)
                {
                    // Handle upgrades
                    if (String.IsNullOrEmpty(location.name) || String.IsNullOrEmpty(location.tz_long))
                    {
                        location.name = weather.location.name;
                        location.tz_long = weather.location.tz_long;

                        await Settings.UpdateLocation(location).ConfigureAwait(false);
                    }
                    if (location.latitude == 0 && location.longitude == 0 &&
                        weather.location.latitude.HasValue && weather.location.longitude.HasValue)
                    {
                        location.latitude = weather.location.latitude.Value;
                        location.longitude = weather.location.longitude.Value;

                        await Settings.UpdateLocation(location).ConfigureAwait(false);
                    }
                    if (String.IsNullOrWhiteSpace(location.locationSource))
                    {
                        location.locationSource = wm.LocationProvider.LocationAPI;
                        await Settings.UpdateLocation(location).ConfigureAwait(false);
                    }

                    if (!loadedSavedData)
                    {
                        await SaveWeatherData().ConfigureAwait(false);
                        await SaveWeatherForecasts().ConfigureAwait(false);
                    }

                    if ((request.LoadAlerts || weather.weather_alerts != null) && wm.SupportsAlerts)
                    {
                        weatherAlerts = weather.weather_alerts;
                        if (!loadedSavedAlertData)
                        {
                            await SaveWeatherAlerts().ConfigureAwait(false);
                        }
                    }
                }
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

            return WeatherResult.Create(weather, !loadedSavedData);
        }

        /// <summary>
        /// _LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        private async Task<WeatherResult> _LoadWeatherData(WeatherRequest request)
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
             */

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Loading weather data for {1}", TAG, location?.ToString());

            bool gotData = await LoadSavedWeatherData(request).ConfigureAwait(false);

            if (!gotData)
            {
                if (request.ShouldSaveData)
                {
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Saved weather data invalid for {1}", TAG, location?.ToString());
                    Logger.WriteLine(LoggerLevel.Debug, "{0}: Retrieving data from weather provider", TAG);

                    if ((weather != null && weather.source != Settings.API)
                        || (weather == null && location != null && location.weatherSource != Settings.API))
                    {
                        // Only update location data if location region is supported by new API
                        // If not don't update so we can use fallback (previously used API)
                        if (wm.IsRegionSupported(location.country_code))
                        {
                            // Update location query and source for new API
                            string oldKey = location.query;

                            if (weather != null)
                            {
                                if (weather.location?.latitude == null || weather.location?.longitude == null)
                                {
                                    throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                                }

                                location.query = wm.UpdateLocationQuery(weather);
                            }
                            else
                            {
                                if (location.latitude == 0.0 || location.longitude == 0.0)
                                {
                                    throw new WeatherException(WeatherUtils.ErrorStatus.Unknown);
                                }

                                location.query = wm.UpdateLocationQuery(location);
                            }

                            location.weatherSource = Settings.API;

                            // Update database as well
                            if (location.locationType == LocationType.GPS)
                            {
                                Settings.SaveLastGPSLocData(location);
                                SimpleLibrary.GetInstance().RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE);
                            }
                            else
                            {
                                await Settings.UpdateLocationWithKey(location, oldKey).ConfigureAwait(false);
#if WINDOWS_UWP && !UNIT_TEST
                                // Update tile id for location
                                SimpleLibrary.GetInstance().RequestAction(
                                    CommonActions.ACTION_WEATHER_UPDATETILELOCATION,
                                    new Dictionary<string, object>
                                    {
                                            { Constants.TILEKEY_OLDKEY, oldKey },
                                            { Constants.TILEKEY_LOCATION, location.query },
                                    });
#endif
                            }
                        }
                    }
                }

                return await GetWeatherData(request).ConfigureAwait(false);
            }
            else
            {
                return WeatherResult.Create(weather, false);
            }
        }

        private Task<bool> LoadSavedWeatherData(WeatherRequest request)
        {
            return LoadSavedWeatherData(request, false);
        }

        private async Task<bool> LoadSavedWeatherData(WeatherRequest request, bool _override)
        {
            // Load weather data
            try
            {
                request.ThrowIfCancellationRequested();

                weather = await Settings.GetWeatherData(location.query);

                if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                    weatherAlerts = weather.weather_alerts = await Settings.GetWeatherAlertData(location.query).ConfigureAwait(false);

                request.ThrowIfCancellationRequested();

                if (request.LoadForecasts && weather != null)
                {
                    var forecasts = await Settings.GetWeatherForecastData(location.query).ConfigureAwait(false);
                    var hrForecasts = await Settings.GetHourlyWeatherForecastData(location.query).ConfigureAwait(false);
                    weather.forecast = forecasts?.forecast;
                    weather.hr_forecast = hrForecasts;
                    weather.txt_forecast = forecasts?.txt_forecast;
                }

                request.ThrowIfCancellationRequested();

                if (_override && weather == null)
                {
                    // If weather is still unavailable try manually searching for it
                    weather = await Settings.GetWeatherDataByCoordinate(location).ConfigureAwait(false);

                    if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                        weatherAlerts = weather.weather_alerts = await Settings.GetWeatherAlertData(weather.query).ConfigureAwait(false);

                    request.ThrowIfCancellationRequested();

                    if (request.LoadForecasts && weather != null)
                    {
                        var forecasts = await Settings.GetWeatherForecastData(location.query).ConfigureAwait(false);
                        var hrForecasts = await Settings.GetHourlyWeatherForecastData(location.query).ConfigureAwait(false);
                        weather.forecast = forecasts?.forecast;
                        weather.hr_forecast = hrForecasts;
                        weather.txt_forecast = forecasts?.txt_forecast;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error loading saved weather data");
                weather = null;
                throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }

            return IsDataValid(_override);
        }

        private async Task CheckForOutdatedObservation()
        {
            if (weather != null)
            {
                // Check for outdated observation
                var now = DateTimeOffset.Now.ToOffset(location.tz_offset);
                var durationMins = weather?.condition?.observation_time == null ? 61 : (now - weather.condition.observation_time).TotalMinutes;
                if (durationMins > 60)
                {
                    var interval = WeatherManager.GetProvider(weather.source).HourlyForecastInterval;

                    var nowHour = now.Trim(TimeSpan.TicksPerHour);
                    var hrf = await Settings.GetFirstHourlyWeatherForecastDataByDate(location.query, nowHour.ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture)).ConfigureAwait(false);
                    if (hrf == null || ((hrf.date - now) is TimeSpan dur && dur.TotalHours > (long)(interval * 0.5)))
                    {
                        var prevHrf = await Settings.GetFirstHourlyWeatherForecastDataByDate(location.query, nowHour.AddHours(-interval).Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture)).ConfigureAwait(false);
                        if (prevHrf != null) hrf = prevHrf;
                    }

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
                            weather.condition.beaufort = new Beaufort(WeatherUtils.GetBeaufortScale((int)Math.Round(hrf.wind_mph.Value)));
                        }
                        weather.condition.feelslike_f = hrf.extras?.feelslike_f ?? 0.0f;
                        weather.condition.feelslike_c = hrf.extras?.feelslike_c ?? 0.0f;
                        weather.condition.uv = hrf.extras?.uv_index.HasValue == true && hrf.extras?.uv_index >= 0 ? new UV(hrf.extras.uv_index.Value) : null;

                        weather.condition.observation_time = hrf.date;

                        if (durationMins > 60 * 6)
                        {
                            var fcasts = await Settings.GetWeatherForecastData(location.query);
                            var fcast = fcasts?.forecast?.FirstOrDefault(f => f.date.Date == now.Date);

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
                            weather.precipitation.cloudiness = hrf.extras?.cloudiness;
                            weather.precipitation.qpf_rain_in = hrf.extras?.qpf_rain_in >= 0 ? hrf.extras.qpf_rain_in : 0.0f;
                            weather.precipitation.qpf_rain_mm = hrf.extras?.qpf_rain_mm >= 0 ? hrf.extras.qpf_rain_mm : 0.0f;
                            weather.precipitation.qpf_snow_in = hrf.extras?.qpf_snow_in >= 0 ? hrf.extras.qpf_snow_in : 0.0f;
                            weather.precipitation.qpf_snow_cm = hrf.extras?.qpf_snow_cm >= 0 ? hrf.extras.qpf_snow_cm : 0.0f;
                        }

                        await Settings.SaveWeatherData(weather).ConfigureAwait(false);
                    }
                }

                if (weather.forecast?.Count > 0)
                {
                    weather.forecast = weather.forecast.Where(f => f.date.Trim(TimeSpan.TicksPerDay) >= now.Date.Trim(TimeSpan.TicksPerDay)).ToList();
                }

                if (weather.hr_forecast?.Count > 0)
                {
                    weather.hr_forecast = weather.hr_forecast.Where(f => f.date.Trim(TimeSpan.TicksPerHour) >= now.Trim(TimeSpan.TicksPerHour)).ToList();
                }
            }
        }

        private bool IsDataValid(bool _override)
        {
            var culture = CultureUtils.UserCulture;
            var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            String API = Settings.API;
            bool isInvalid = weather == null || !weather.IsValid();
            if (!isInvalid && !String.Equals(weather.source, API))
            {
                // Don't mark data as invalid if region is not supported
                // This is so we can use the fallback, if location data was not already modified
                if (wm.IsRegionSupported(location.country_code))
                    isInvalid = true;
            }

            if (wm.SupportsWeatherLocale && !isInvalid)
                isInvalid = !String.Equals(weather.locale, locale);

            if (_override || isInvalid) return !isInvalid;

            // Weather data expiration
            int ttl;
            // TODO: make this a premium feature
            if (WeatherAPI.Here.Equals(wm.WeatherAPI))
            {
                ttl = Settings.RefreshInterval;
            }
            else
            {
                ttl = Math.Max(weather.ttl, Settings.RefreshInterval);
            }

            // Check file age
            DateTimeOffset updateTime = weather.update_time;

            TimeSpan span = DateTimeOffset.Now - updateTime;
            return span.TotalMinutes < ttl;
        }

        private async Task SaveWeatherData()
        {
            // Save location query
            weather.query = location.query;

            await Settings.SaveWeatherData(weather).ConfigureAwait(false);
        }

        private async Task SaveWeatherAlerts()
        {
            if (weatherAlerts != null)
            {
                // Check for previously saved alerts
                var previousAlerts = await Settings.GetWeatherAlertData(location.query).ConfigureAwait(false);

                if (previousAlerts.Any())
                {
                    // If any previous alerts were flagged before as notified
                    // make sure to set them here as such
                    // bc notified flag gets reset when retrieving weatherdata
                    foreach (WeatherAlert alert in weatherAlerts)
                    {
                        if (previousAlerts.FirstOrDefault(walert => walert.Equals(alert)) is WeatherAlert prevAlert)
                        {
                            if (prevAlert.Notified)
                                alert.Notified = prevAlert.Notified;
                        }
                    }
                }

                await Settings.SaveWeatherAlerts(location, weatherAlerts).ConfigureAwait(false);
            };
        }

        private async Task SaveWeatherForecasts()
        {
            await Settings.SaveWeatherForecasts(new Forecasts(weather)).ConfigureAwait(false);
            await Settings.SaveWeatherForecasts(location, weather?.hr_forecast?.Select(f => new HourlyForecasts(weather?.query, f))).ConfigureAwait(false);
        }
    }
}