using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.TZDB;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleWeather.Common.WeatherData
{
    public sealed class WeatherDataLoader
    {
        private LocationData.LocationData location;
        private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        private readonly ITZDBService TZDBService = Ioc.Default.GetService<ITZDBService>();

        private const String TAG = nameof(WeatherDataLoader);

        public WeatherDataLoader()
        {
            this.location = new LocationData.LocationData();
        }

        public WeatherDataLoader(LocationData.LocationData location) : this()
        {
            UpdateLocation(location);
        }

        public bool IsLocationValid()
        {
            return location?.IsValid() == true;
        }

        public void UpdateLocation(LocationData.LocationData locationData)
        {
            if (locationData == null || !locationData.IsValid())
            {
                Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(locationData));
                throw new ArgumentException(nameof(locationData));
            }

            this.location = locationData;
        }

        /// <summary>
        /// LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        public async Task<Weather> LoadWeatherData(WeatherRequest request)
        {
            var result = await LoadWeatherTask(request);

            return result switch
            {
                WeatherResult.Success => result.Data,
                WeatherResult.NoWeather => null,
                WeatherResult.Error werr => throw werr.Exception,
                WeatherResult.WeatherWithError werr => throw werr.Exception
            };
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
            WeatherResult result;

            try
            {
                if (request.ForceLoadSavedData)
                {
                    result = await LoadSavedWeatherData(request, true).ConfigureAwait(false);
                }
                else
                {
                    if (request.ForceRefresh)
                    {
                        result = await GetWeatherData(request).ConfigureAwait(false);
                    }
                    else
                    {
                        result = await LoadWeatherDataInternal(request).ConfigureAwait(false);
                    }
                }

                if (result.Data != null)
                {
                    await CheckForOutdatedObservation(result.Data, request);
                }
            }
            catch (WeatherException wEx)
            {
                return new WeatherResult.Error(wEx);
            }

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Weather data for {1} is valid = {2}", TAG, location?.ToString(), result.Data?.IsValid());

            return result;
        };

        public async Task<ICollection<WeatherAlert>> LoadWeatherAlerts(bool loadSavedData)
        {
            ICollection<WeatherAlert> weatherAlerts = null;

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
                    weatherAlerts = await SettingsManager.GetWeatherAlertData(location.query).ConfigureAwait(false);
                }

                if (!loadSavedData)
                {
                    await SaveWeatherAlerts(weatherAlerts).ConfigureAwait(false);
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
            bool loadedSavedAlertData = false;
            Weather weather = null;

            // Try to get weather from provider API
            try
            {
                request.ThrowIfCancellationRequested();

                // Is the timezone valid? If not try to fetch a valid zone id
                if (!wm.IsRegionSupported(location) && (string.IsNullOrWhiteSpace(location.tz_long) || Equals(location.tz_long, "unknown") || Equals(location.tz_long, "UTC")))
                {
                    if (location.latitude != 0 && location.longitude != 0)
                    {
                        var tzId = await TZDBService.GetTimeZone(location.latitude, location.longitude);
                        if (!Equals("unknown", tzId))
                        {
                            location.tz_long = tzId;
                            // Update DB here or somewhere else
                            await SettingsManager.UpdateLocation(location);
                        }
                    }
                }

                if (!wm.IsRegionSupported(location))
                {
                    if (location.latitude != 0 && location.longitude != 0)
                    {
                        // If location data hasn't been updated, try loading weather from the previous provider
                        if (!String.IsNullOrWhiteSpace(location.weatherSource))
                        {
                            var provider = wm.GetWeatherProvider(location.weatherSource);
                            if (provider.IsRegionSupported(location))
                            {
                                weather = await provider.GetWeather(location).ConfigureAwait(false);
                            }
                        }

                        // Nothing to fallback on; error out
                        if (weather == null)
                        {
                            Logger.WriteLine(LoggerLevel.Warn, "Location: {0}", JSONParser.Serializer(location));
                            throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound, new InvalidOperationException("Invalid location data"));
                        }
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
                throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex);
            }

            if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
            {
                if (wm.NeedsExternalAlertData)
                {
                    weather.weather_alerts = await wm.GetAlerts(location).ConfigureAwait(false);
                }

                if (weather.weather_alerts == null)
                {
                    weather.weather_alerts = await SettingsManager.GetWeatherAlertData(location.query).ConfigureAwait(false);
                    loadedSavedAlertData = true;
                }
            }

            if (request.ShouldSaveData)
            {
                if (weather != null)
                {
                    // Handle upgrades
                    if (String.IsNullOrEmpty(location.name) || String.IsNullOrEmpty(location.tz_long))
                    {
                        location.name = weather.location.name;
                        location.tz_long = weather.location.tz_long;

                        await SettingsManager.UpdateLocation(location).ConfigureAwait(false);
                    }

                    if (location.latitude == 0 && location.longitude == 0 &&
                        weather.location.latitude.GetValueOrDefault(0f) != 0f &&
                        weather.location.longitude.GetValueOrDefault(0f) != 0f)
                    {
                        location.latitude = weather.location.latitude.Value;
                        location.longitude = weather.location.longitude.Value;

                        await SettingsManager.UpdateLocation(location).ConfigureAwait(false);
                    }

                    if (String.IsNullOrWhiteSpace(location.locationSource))
                    {
                        location.locationSource = wm.LocationProvider.LocationAPI;

                        await SettingsManager.UpdateLocation(location).ConfigureAwait(false);
                    }

                    await SaveWeatherData(weather).ConfigureAwait(false);
                    await SaveWeatherForecasts(weather).ConfigureAwait(false);

                    if ((request.LoadAlerts || weather.weather_alerts != null) && wm.SupportsAlerts)
                    {
                        if (!loadedSavedAlertData)
                        {
                            await SaveWeatherAlerts(weather.weather_alerts).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    // Load old data if available and we can't get new data
                    var result = await LoadSavedWeatherData(request, true).ConfigureAwait(false);

                    if (wEx != null)
                    {
                        if (result.Data != null)
                        {
                            return new WeatherResult.WeatherWithError(result.Data, wEx, true);
                        }
                        else
                        {
                            return new WeatherResult.Error(wEx);
                        }
                    }
                }
            }

            // Throw exception if we're unable to get any weather data
            if (wEx != null)
            {
                throw wEx;
            }
            else if (weather == null)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }

            return weather.ToWeatherResult(false);
        }

        /// <summary>
        /// _LoadWeatherData
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException"></exception>
        private async Task<WeatherResult> LoadWeatherDataInternal(WeatherRequest request)
        {
            /*
             * If unable to retrieve saved data, data is old, or units don't match
             * Refresh weather data
             */

            Logger.WriteLine(LoggerLevel.Debug, "{0}: Loading weather data for {1}", TAG, location?.ToString());

            var result = await LoadSavedWeatherData(request).ConfigureAwait(false);

            switch (result)
            {
                case WeatherResult.NoWeather:
                    {
                        if (request.ShouldSaveData)
                        {
                            Logger.WriteLine(LoggerLevel.Debug, "{0}: Saved weather data invalid for {1}", TAG, location?.ToString());
                            Logger.WriteLine(LoggerLevel.Debug, "{0}: Retrieving data from weather provider", TAG);
                            var weather = result.Data;

                            if (weather != null && weather.source != SettingsManager.API || location.weatherSource != SettingsManager.API)
                            {
                                // Only update location data if location region is supported by new API
                                // If not don't update so we can use fallback (previously used API)
                                if (wm.IsRegionSupported(location))
                                {
                                    // Update location query and source for new API
                                    string oldKey = location.query;

                                    if (location.latitude != 0.0 || location.longitude != 0.0)
                                    {
                                        location.query = wm.UpdateLocationQuery(location);
                                    }
                                    else if (weather != null)
                                    {
                                        if (weather.location?.latitude.GetValueOrDefault(0f) != 0f || weather.location?.longitude.GetValueOrDefault(0f) != 0f)
                                        {
                                            throw new WeatherException(WeatherUtils.ErrorStatus.Unknown, new Exception($"Invalid location data: {weather.location}"));
                                        }

                                        location.query = wm.UpdateLocationQuery(weather);
                                    }
                                    else
                                    {
                                        throw new WeatherException(WeatherUtils.ErrorStatus.Unknown, new Exception($"Invalid location state: location ({location}), weather ({weather})"));
                                    }

                                    location.weatherSource = SettingsManager.API;

                                    // Update database as well
                                    if (location.locationType == LocationType.GPS)
                                    {
                                        await SettingsManager.SaveLastGPSLocData(location);
                                        SharedModule.Instance.RequestAction(CommonActions.ACTION_WEATHER_SENDLOCATIONUPDATE);
                                    }
                                    else
                                    {
                                        await SettingsManager.UpdateLocationWithKey(location, oldKey).ConfigureAwait(false);
#if (WINDOWS || __IOS__) && !UNIT_TEST
                                        // Update widget id for location
                                        SharedModule.Instance.RequestAction(
                                            CommonActions.ACTION_WEATHER_UPDATEWIDGETLOCATION,
                                            new Dictionary<string, object>
                                            {
                                                { Constants.WIDGETKEY_OLDKEY, oldKey },
                                                { Constants.WIDGETKEY_LOCATION, location },
                                            });
#endif
                                    }
                                }
                            }
                        }

                        return await GetWeatherData(request).ConfigureAwait(false);
                    }
                default:
                    return result;
            }
        }

        private async Task<WeatherResult> LoadSavedWeatherData(WeatherRequest request, bool _override = false)
        {
            Weather weather;

            // Load weather data
            try
            {
                request.ThrowIfCancellationRequested();

                weather = await SettingsManager.GetWeatherData(location.query);

                if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                {
                    weather.weather_alerts = await SettingsManager.GetWeatherAlertData(location.query).ConfigureAwait(false);
                }

                request.ThrowIfCancellationRequested();

                if (request.LoadForecasts && weather != null)
                {
                    var forecasts = await SettingsManager.GetWeatherForecastData(location.query).ConfigureAwait(false);
                    var hrForecasts = await SettingsManager.GetHourlyWeatherForecastData(location.query).ConfigureAwait(false);
                    weather.forecast = forecasts?.forecast;
                    weather.hr_forecast = hrForecasts;
                    weather.txt_forecast = forecasts?.txt_forecast;
                }

                request.ThrowIfCancellationRequested();

                if (_override && weather == null)
                {
                    // If weather is still unavailable try manually searching for it
                    weather = await SettingsManager.GetWeatherDataByCoordinate(location).ConfigureAwait(false);

                    if (request.LoadAlerts && weather != null && wm.SupportsAlerts)
                    {
                        weather.weather_alerts = await SettingsManager.GetWeatherAlertData(weather.query).ConfigureAwait(false);
                    }

                    request.ThrowIfCancellationRequested();

                    if (request.LoadForecasts && weather != null)
                    {
                        var forecasts = await SettingsManager.GetWeatherForecastData(location.query).ConfigureAwait(false);
                        var hrForecasts = await SettingsManager.GetHourlyWeatherForecastData(location.query).ConfigureAwait(false);
                        weather.forecast = forecasts?.forecast;
                        weather.hr_forecast = hrForecasts;
                        weather.txt_forecast = forecasts?.txt_forecast;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherDataLoader: error loading saved weather data");
                return new WeatherResult.Error(new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex));
            }

            if (IsDataValid(weather, _override))
            {
                return new WeatherResult.Success(weather, true);
            }
            else
            {
                return new WeatherResult.NoWeather(weather, true);
            }
        }
        private async Task CheckForOutdatedObservation(Weather weather, WeatherRequest request)
        {
            if (weather != null)
            {
                // Check for outdated observation
                var now = DateTimeOffset.Now.ToOffset(location.tz_offset);
                var durationMins = weather?.condition?.observation_time == null ? 61 : (now - weather.condition.observation_time).TotalMinutes;
                if (durationMins > 90)
                {
                    var interval = wm.GetWeatherProvider(weather.source).HourlyForecastInterval;

                    var nowHour = now.Trim(TimeSpan.TicksPerHour);
                    var hrf = await SettingsManager.GetFirstHourlyWeatherForecastDataByDate(location.query, nowHour).ConfigureAwait(false);
                    if (hrf == null || ((hrf.date - now) is TimeSpan dur && dur.TotalHours > (long)(interval * 0.5)))
                    {
                        var prevHrf = await SettingsManager.GetFirstHourlyWeatherForecastDataByDate(location.query, nowHour.AddHours(-interval).Trim(TimeSpan.TicksPerHour)).ConfigureAwait(false);
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
                            var fcasts = await SettingsManager.GetWeatherForecastData(location.query);
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

                        await SaveWeatherData(weather).ConfigureAwait(false);
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

        private bool IsDataValid(Weather weather, bool overrideTtl = false)
        {
            var culture = LocaleUtils.GetLocale();
            var locale = wm.LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            if (weather == null) return false;

            var isInvalid = !weather.IsValid();
            String API = SettingsManager.API;

            if (!isInvalid && !String.Equals(weather.source, API))
            {
                isInvalid = true;
            }

            if (!isInvalid && wm.SupportsWeatherLocale)
            {
                isInvalid = !String.Equals(weather.locale, locale);
            }

            if (overrideTtl || isInvalid) return !isInvalid;

            // Weather data expiration
            int ttl = Math.Max(SettingsManager.RefreshInterval, 30);

            // Check file age
            TimeSpan span = DateTimeOffset.Now - weather.update_time;
            return span.TotalMinutes < ttl;
        }

        private async Task SaveWeatherData(Weather weather)
        {
            // Save location query
            weather.query = location.query;

            await SettingsManager.SaveWeatherData(weather).ConfigureAwait(false);
        }

        private async Task SaveWeatherAlerts(ICollection<WeatherAlert> weatherAlerts)
        {
            // Check for previously saved alerts
            var previousAlerts = await SettingsManager.GetWeatherAlertData(location.query).ConfigureAwait(false);

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

            await SettingsManager.SaveWeatherAlerts(location, weatherAlerts).ConfigureAwait(false);
        }

        private async Task SaveWeatherForecasts(Weather weather)
        {
            if (weather != null)
            {
                var forecasts = new Forecasts(weather);
                await SettingsManager.SaveWeatherForecasts(forecasts).ConfigureAwait(false);
                var hrForecasts = weather?.hr_forecast?.Select(f => new HourlyForecasts(weather?.query, f));

                await SettingsManager.SaveWeatherForecasts(location.query, hrForecasts).ConfigureAwait(false);
            }
        }
    }
}