using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Bing;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.WeatherApi
{
    public partial class WeatherApiProvider : WeatherProviderImpl, IWeatherAlertProvider
    {
        private const String BASE_URL = "https://api.weatherapi.com/v1/";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "forecast.json?key={0}";

        private const String WEATHER_QUERY_URL =
            BASE_URL + "forecast.json?q={0}&days=10&aqi=yes&alerts=yes&lang={1}&key={2}";

        private const String ALERTS_QUERY_URL =
            BASE_URL + "forecast.json?q={0}&days=1&hour=6&aqi=no&alerts=yes&lang={1}&key={2}";

        public WeatherApiProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new WeatherApiLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.WeatherApi;
        public override bool SupportsWeatherLocale => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => true;
        public override int HourlyForecastInterval => 1;
        public override AuthType AuthType => AuthType.ApiKey;

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
                var webClient = SharedModule.Instance.WebClient;
                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                using (var response = await webClient.GetAsync(queryURL, cts.Token))
                {
                    // Check for errors
                    await this.ThrowIfRateLimited(response);
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
            return APIKeys.GetWeatherApiKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = LocaleUtils.GetLocale();

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetProviderKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            var query = await UpdateLocationQuery(location);

            try
            {
                this.CheckRateLimit();

                Uri weatherURL = new Uri(string.Format(WEATHER_QUERY_URL, query, locale, key));

                using var request = new HttpRequestMessage(HttpMethod.Get, weatherURL);
                request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(20));

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var response = await webClient.SendAsync(request, cts.Token);
                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                Stream stream = await response.Content.ReadAsStreamAsync();

                // Load weather
                var root = await JSONParser.DeserializerAsync<ForecastRootobject>(stream);

                weather = this.CreateWeatherData(root);
            }
            catch (Exception ex)
            {
                weather = null;

                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                if (SupportsWeatherLocale)
                    weather.locale = locale;

                weather.query = query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public override async Task<ICollection<WeatherAlert>> GetAlerts(
            SimpleWeather.LocationData.LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            var culture = LocaleUtils.GetLocale();

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = GetProviderKey();

            try
            {
                this.CheckRateLimit();

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                Uri alertsURL =
                    new Uri(string.Format(ALERTS_QUERY_URL, await UpdateLocationQuery(location), locale, key));

                using (var request = new HttpRequestMessage(HttpMethod.Get, alertsURL))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        var root = await JSONParser.DeserializerAsync<ForecastRootobject>(stream);

                        if (root.alerts?.alert?.Length > 0)
                        {
                            alerts = new List<WeatherAlert>(root.alerts.alert.Length);

                            foreach (var alert in root.alerts.alert)
                            {
                                alerts.Add(this.CreateWeatherAlert(alert));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherApiProvider: error getting weather alert data");
            }

            if (alerts == null)
            {
                alerts = new List<WeatherAlert>(0);
            }

            return alerts;
        }

        protected override Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location, Weather weather)
        {
            // no-op
            return Task.CompletedTask;
        }

        public override Task<string> UpdateLocationQuery(Weather weather)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}",
                location.latitude, location.longitude));
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            String code = "en";

            switch (iso)
            {
                // Chinese
                case "zh":
                    switch (name)
                    {
                        // Chinese - Traditional
                        case "zh-Hant":
                        case "zh-HK":
                        case "zh-MO":
                        case "zh-TW":
                            code = "zh_tw";
                            break;
                        // Mandarin
                        case "zh-cmn":
                            code = "zh_cmn";
                            break;
                        // Wu
                        case "zh-wuu":
                            code = "zh_wuu";
                            break;
                        // Xiang
                        case "zh-hsn":
                            code = "zh_hsn";
                            break;
                        // Cantonese
                        case "zh-yue":
                            code = "zh_yue";
                            break;
                        // Chinese - Simplified
                        default:
                            code = "zh";
                            break;
                    }

                    break;
                default:
                    code = iso;
                    break;
            }

            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null || !int.TryParse(icon, out int conditionCode))
                return WeatherIcons.NA;

            string WeatherIcon = conditionCode switch
            {
                /* Sunny / Clear */
                1000 => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                /* Partly cloudy */
                1003 => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,

                /* Cloudy */
                1006 => WeatherIcons.CLOUDY,

                /* Overcast */
                1009 => WeatherIcons.OVERCAST,

                /*
                 * 1030: Mist
                 * 1135: Fog
                 * 1147: Freezing fog
                 */
                1030 or 1135 or 1147 => WeatherIcons.FOG,

                /*
                 * 1063: Patchy rain possible
                 * 1186: Moderate rain at times
                 */
                1063 or 1186 => isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN,

                /*
                 * 1066: Patchy snow possible
                 * 1210: Patchy light snow
                 */
                1066 or 1210 => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,

                /*
                 * 1069: Patchy sleet possible
                 * 1249: Light sleet showers
                 * 1252: Moderate or heavy sleet showers
                 */
                1069 or 1249 or 1252 => isNight ? WeatherIcons.NIGHT_ALT_SLEET : WeatherIcons.DAY_SLEET,

                /*
                 * 1072: Patchy freezing drizzle possible
                 */
                1072 => WeatherIcons.RAIN_MIX,

                /* Thundery outbreaks possible */
                1087 => isNight ? WeatherIcons.NIGHT_ALT_LIGHTNING : WeatherIcons.DAY_LIGHTNING,

                /*
                 * 1114: Blowing snow
                 * 1117: Blizzard
                 * 1225: Heavy snow
                 */
                1114 or 1117 or 1225 => WeatherIcons.SNOW_WIND,

                /*
                 * 1213: Light snow
                 * 1219: Moderate snow
                 */
                1213 or 1219 => WeatherIcons.SNOW,

                /*
                 * 1216: Patchy moderate snow
                 * 1255: Light snow showers
                 */
                1216 or 1255 => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,

                /*
                 * 1222: Patchy heavy snow
                 * 1258: Moderate or heavy snow showers
                 */
                1222 or 1258 => isNight ? WeatherIcons.NIGHT_ALT_SNOW_WIND : WeatherIcons.DAY_SNOW_WIND,

                /*
                 * 1150: Patchy light drizzle
                 * 1153: Light drizzle
                 * 1183: Light rain
                 */
                1150 or 1153 or 1183 => WeatherIcons.SPRINKLE,

                /*
                 * 1168: Freezing drizzle
                 * 1171: Heavy freezing drizzle
                 * 1198: Light freezing rain
                 * 1201: Moderate or heavy freezing rain
                 */
                1168 or 1171 or 1198 or 1201 => WeatherIcons.RAIN_MIX,

                /*
                 * 1180: Patchy light rain
                 */
                1180 => isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE,

                /*
                 * 1189: Moderate rain
                 */
                1189 => WeatherIcons.RAIN,

                /*
                 * 1192: Heavy rain at times
                 * 1243: Moderate or heavy rain shower
                 * 1246: Torrential rain shower
                 */
                1192 or 1243 or 1246 => isNight ? WeatherIcons.NIGHT_ALT_RAIN_WIND : WeatherIcons.DAY_RAIN_WIND,

                /*
                 * 1195: Heavy rain
                 */
                1195 => WeatherIcons.RAIN_WIND,

                /*
                 * 1204: Light sleet
                 * 1207: Moderate or heavy sleet
                 */
                1204 or 1207 => WeatherIcons.SLEET,

                /*
                 * 1237: Ice pellets
                 */
                1237 => WeatherIcons.HAIL,

                /* 1240: Light rain shower */
                1240 => isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS,

                /*
                 * 1261: Light showers of ice pellets
                 * 1264: Moderate or heavy showers of ice pellets
                 */
                1261 or 1264 => isNight ? WeatherIcons.NIGHT_ALT_HAIL : WeatherIcons.DAY_HAIL,

                /* Patchy light rain with thunder */
                1273 => isNight ? WeatherIcons.NIGHT_ALT_STORM_SHOWERS : WeatherIcons.DAY_STORM_SHOWERS,

                /* Moderate or heavy rain with thunder */
                1276 => WeatherIcons.THUNDERSTORM,

                /*
                 * 1279: Patchy light snow with thunder
                 * 1282: Moderate or heavy snow with thunder
                 */
                1279 or 1282 => isNight ? WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM : WeatherIcons.DAY_SNOW_THUNDERSTORM,

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
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
                LocalTime sunrise;
                LocalTime sunset;
                if (weather.astronomy != null)
                {
                    sunrise = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    sunset = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                }
                else
                {
                    sunrise = LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                    sunset = LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                }

                DateTimeZone tz = null;

                if (weather.location.tz_long != null)
                {
                    tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                }

                if (tz == null)
                    tz = DateTimeZone.ForOffset(Offset.FromTimeSpan(weather.location.tz_offset));

                var now = SystemClock.Instance.GetCurrentInstant()
                    .InZone(tz).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }
    }
}