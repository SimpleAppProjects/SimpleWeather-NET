using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.TomorrowIO
{
    public partial class TomorrowIOWeatherProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://api.tomorrow.io/v4/timelines";
        private const String EVENTS_BASE_URL = "https://api.tomorrow.io/v4/events";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "?apikey={0}";

        public TomorrowIOWeatherProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.TomorrowIo;
        public override bool SupportsWeatherLocale => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => true;
        public override int HourlyForecastInterval => 1;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(KEYCHECK_QUERY_URL, key));

                // Connect to webstream
                var webClient = SimpleLibrary.GetInstance().WebClient;
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                using (var response = await webClient.GetAsync(queryURL).AsTask(cts.Token))
                {
                    // Check for errors
                    this.ThrowIfRateLimited(response.StatusCode);
                    switch (response.StatusCode)
                    {
                        // 400 (OK since this isn't a valid request)
                        case HttpStatusCode.BadRequest:
                            isValid = true;
                            break;
                        // 401 (Unauthorized - Key is invalid)
                        case HttpStatusCode.Unauthorized:
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                            isValid = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
            }

            if (wEx != null && wEx.ErrorStatus != WeatherUtils.ErrorStatus.InvalidAPIKey)
            {
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Rootobject root;
                Rootobject minutelyRoot = null;
                AlertRootobject alertsRoot = null;

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("fields", "temperature,temperatureApparent,temperatureMin,temperatureMax,dewPoint,humidity,windSpeed,windDirection,windGust,pressureSeaLevel,precipitationIntensity,precipitationProbability,snowAccumulation,sunriseTime,sunsetTime,visibility,cloudCover,moonPhase,weatherCode,treeIndex,grassIndex,weedIndex,epaIndex")
                    .AppendQueryParameter("timesteps", "current,1h,1d")
                    .AppendQueryParameter("units", "metric")
                    .AppendQueryParameter("timezone", "UTC")
                    .BuildUri();

                var minutelyRequestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("fields", "precipitationIntensity,precipitationProbability")
                    .AppendQueryParameter("timesteps", "1m")
                    .AppendQueryParameter("units", "metric")
                    .AppendQueryParameter("timezone", "UTC")
                    .BuildUri();

                var alertsRequestUri = EVENTS_BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("insights", "air,fires,wind,winter,thunderstorms,floods,temperature,tropical,marine,fog,tornado")
                    .AppendQueryParameter("buffer", "20")
                    .BuildUri();

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    request.Headers.CacheControl.MaxAge = TimeSpan.FromHours(3);
                    request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                    using var response = await webClient.SendRequestAsync(request).AsTask(cts.Token);

                    this.CheckForErrors(response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    using var stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // Load weather
                    root = await JSONParser.DeserializerAsync<Rootobject>(stream);
                }

                try
                {
                    using (var minutelyRequest = new HttpRequestMessage(HttpMethod.Get, minutelyRequestUri))
                    {
                        minutelyRequest.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(45);
                        minutelyRequest.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                        // Get response
                        var webClient = SimpleLibrary.GetInstance().WebClient;
                        using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                        using var response = await webClient.SendRequestAsync(minutelyRequest).AsTask(cts.Token);

                        this.CheckForErrors(response.StatusCode);
                        response.EnsureSuccessStatusCode();

                        using var stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load weather
                        minutelyRoot = await JSONParser.DeserializerAsync<Rootobject>(stream);
                    }
                }
                catch { }

                try
                {
                    using (var alertsRequest = new HttpRequestMessage(HttpMethod.Get, alertsRequestUri))
                    {
                        alertsRequest.Headers.CacheControl.MaxAge = TimeSpan.FromHours(6);
                        alertsRequest.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                        // Get response
                        var webClient = SimpleLibrary.GetInstance().WebClient;
                        using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                        using var response = await webClient.SendRequestAsync(alertsRequest).AsTask(cts.Token);

                        this.CheckForErrors(response.StatusCode);
                        response.EnsureSuccessStatusCode();

                        using var stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                        // Load weather
                        alertsRoot = await JSONParser.DeserializerAsync<AlertRootobject>(stream);
                    }
                }
                catch { }

                weather = new Weather(root, minutelyRoot, alertsRoot);
            }
            catch (Exception ex)
            {
                weather = null;

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "TomorrowIOWeatherProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                if (SupportsWeatherLocale)
                    weather.locale = locale;

                weather.query = location_query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        protected override async Task UpdateWeatherData(LocationData location, WeatherData.Weather weather)
        {
            var offset = location.tz_offset;

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            var newAstro = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            if (weather.astronomy == null)
            {
                weather.astronomy = newAstro;
            }
            else
            {
                weather.astronomy.sunrise = weather.astronomy.sunrise.AddSeconds(offset.TotalSeconds);
                weather.astronomy.sunset = weather.astronomy.sunset.AddSeconds(offset.TotalSeconds);
                weather.astronomy.moonrise = newAstro.moonrise;
                weather.astronomy.moonset = newAstro.moonset;
            }

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.weather = GetWeatherCondition(weather.condition.icon);
            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.AddSeconds(offset.TotalSeconds);
                forecast.condition = GetWeatherCondition(forecast.icon);
                forecast.icon = GetWeatherIcon(forecast.icon);
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.DateTime.TimeOfDay;
                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (var alert in weather.weather_alerts)
                {
                    if (!alert.Date.Offset.Equals(offset))
                    {
                        alert.Date = new DateTimeOffset(alert.Date.DateTime, offset);
                    }

                    if (!alert.ExpiresDate.Offset.Equals(offset))
                    {
                        alert.ExpiresDate = new DateTimeOffset(alert.ExpiresDate.DateTime, offset);
                    }
                }
            }
        }

        public override string UpdateLocationQuery(WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", location.latitude, location.longitude);
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (icon == null || !int.TryParse(icon, out int conditionCode))
                return WeatherIcons.NA;

            switch (conditionCode)
            {
                /* Sunny / Clear */
                case 1000:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
                    break;

                /*
                 * 1101: Partly cloudy
                 * 1100: Mostly Clear
                 */
                case 1101:
                case 1100:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                /*
                 * 1102: Mostly Cloudy
                 * 1001: Cloudy
                 */
                case 1102:
                case 1001:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY;
                    break;

                /*
                 * 2000: Fog
                 * 2100: Light fog
                 */
                case 2000:
                case 2100:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG;
                    break;

                /* Thunderstorm  */
                case 8000:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM;
                    break;

                /*
                 * 5001: Flurries
                 * 5100: Patchy light snow
                 * 5000: Light snow
                 */
                case 5001:
                case 5100:
                case 5000:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
                    break;

                /* Heavy snow */
                case 5101:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW_WIND : WeatherIcons.DAY_SNOW_WIND;
                    break;

                /*
                 * 7102: Light Ice pellets
                 * 7000: Ice pellets
                 * 7101: Heavy Ice pellets
                 */
                case 7102:
                case 7000:
                case 7101:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_HAIL : WeatherIcons.DAY_HAIL;
                    break;

                /*
                 * 4000: Drizzle
                 * 4200: Light rain
                 */
                case 4000:
                case 4200:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE;
                    break;

                /*
                 * 6000: Freezing drizzle
                 * 6200: Light freezing rain
                 * 6001: Freezing rain
                 * 6201: Heavy freezing drizzle
                 */
                case 6000:
                case 6200:
                case 6001:
                case 6201:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX;
                    break;

                /* Rain */
                case 4001:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN;
                    break;

                /* Heavy rain */
                case 4201:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_WIND : WeatherIcons.DAY_RAIN_WIND;
                    break;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                NodaTime.LocalTime sunrise;
                NodaTime.LocalTime sunset;
                if (weather.astronomy != null)
                {
                    sunrise = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    sunset = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                }
                else
                {
                    sunrise = NodaTime.LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                    sunset = NodaTime.LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                }

                NodaTime.DateTimeZone tz = null;

                if (weather.location.tz_long != null)
                {
                    tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                }

                if (tz == null)
                    tz = NodaTime.DateTimeZone.ForOffset(NodaTime.Offset.FromTimeSpan(weather.location.tz_offset));

                var now = NodaTime.SystemClock.Instance.GetCurrentInstant()
                            .InZone(tz).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }
    }
}