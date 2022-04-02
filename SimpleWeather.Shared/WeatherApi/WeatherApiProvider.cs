using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Http.Headers;
using System.Net;

namespace SimpleWeather.WeatherApi
{
    public partial class WeatherApiProvider : WeatherProviderImpl, IWeatherAlertProvider
    {
        private const String BASE_URL = "https://api.weatherapi.com/v1/";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "forecast.json?key={0}";
        private const String WEATHER_QUERY_URL = BASE_URL + "forecast.json?q={0}&days=10&aqi=yes&alerts=yes&lang={1}&key={2}";
        private const String ALERTS_QUERY_URL = BASE_URL + "forecast.json?q={0}&days=1&hour=6&aqi=no&alerts=yes&lang={1}&key={2}";

        public WeatherApiProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.WeatherApi;
        public override bool SupportsWeatherLocale => true;
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

                Uri weatherURL = new Uri(string.Format(WEATHER_QUERY_URL, location_query, locale, key));

                using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue() 
                    {
                        MaxAge = TimeSpan.FromHours(1)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        var root = await JSONParser.DeserializerAsync<ForecastRootobject>(stream);

                        weather = new Weather(root);
                    }
                }
            }
            catch (Exception ex)
            {
                weather = null;

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
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

                weather.query = location_query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public override async Task<ICollection<WeatherAlert>> GetAlerts(LocationData location)
        {
            ICollection<WeatherAlert> alerts = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Uri alertsURL = new Uri(string.Format(ALERTS_QUERY_URL, UpdateLocationQuery(location), locale, key));

                using (var request = new HttpRequestMessage(HttpMethod.Get, alertsURL))
                {
                    request.Headers.CacheControl = new CacheControlHeaderValue() 
                    {
                        MaxAge = TimeSpan.FromHours(6)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
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
                                alerts.Add(new WeatherAlert(alert));
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

        protected override Task UpdateWeatherData(LocationData location, WeatherData.Weather weather)
        {
            // no-op
            return Task.CompletedTask;
        }

        public override string UpdateLocationQuery(WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", location.latitude, location.longitude);
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
            string WeatherIcon = string.Empty;

            if (icon == null || !int.TryParse(icon, out int conditionCode))
                return WeatherIcons.NA;

            switch (conditionCode)
            {
                /* Sunny / Clear */
                case 1000:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
                    break;

                /* Partly cloudy */
                case 1003:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY;
                    break;

                /* Overcast */
                case 1009:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_OVERCAST : WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                /* Cloudy */
                case 1006:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY;
                    break;

                /*
                 * 1030: Mist
                 * 1135: Fog
                 * 1147: Freezing fog
                 */
                case 1030:
                case 1135:
                case 1147:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG;
                    break;

                /*
                 * 1063: Patchy rain possible
                 * 1186: Moderate rain at times
                 * 1189: Moderate rain
                 */
                case 1063:
                case 1186:
                case 1189:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN;
                    break;

                /*
                 * 1066: Patchy snow possible
                 * 1210: Patchy light snow
                 * 1213: Light snow
                 * 1216: Patchy moderate snow
                 * 1219: Moderate snow
                 * 1255: Light snow showers
                 * 1258: Moderate or heavy snow showers
                 */
                case 1066:
                case 1210:
                case 1213:
                case 1216:
                case 1219:
                case 1255:
                case 1258:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
                    break;

                /*
                 * 1069: Patchy sleet possible
                 * 1204: Light sleet
                 * 1207: Moderate or heavy sleet
                 * 1249: Light sleet showers
                 * 1252: Moderate or heavy sleet showers
                 */
                case 1069:
                case 1204:
                case 1207:
                case 1249:
                case 1252:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SLEET : WeatherIcons.DAY_SLEET;
                    break;

                /*
                 * 1072: Patchy freezing drizzle possible
                 * 1168: Freezing drizzle
                 * 1171: Heavy freezing drizzle
                 * 1198: Light freezing rain
                 * 1201: Moderate or heavy freezing rain
                 */
                case 1072:
                case 1168:
                case 1171:
                case 1198:
                case 1201:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX;
                    break;

                /* Thundery outbreaks possible */
                case 1087:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_LIGHTNING : WeatherIcons.DAY_LIGHTNING;
                    break;

                /*
                 * 1114: Blowing snow
                 * 1117: Blizzard
                 * 1222: Patchy heavy snow
                 * 1225: Heavy snow
                 */
                case 1114:
                case 1117:
                case 1222:
                case 1225:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW_WIND : WeatherIcons.DAY_SNOW_WIND;
                    break;

                /*
                 * 1150: Patchy light drizzle
                 * 1153: Light drizzle
                 * 1180: Patchy light rain
                 * 1183: Light rain
                 */
                case 1150:
                case 1153:
                case 1180:
                case 1183:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE;
                    break;

                /*
                 * 1192: Heavy rain at times
                 * 1195: Heavy rain
                 * 1243: Moderate or heavy rain shower
                 * 1246: Torrential rain shower
                 */
                case 1192:
                case 1195:
                case 1243:
                case 1246:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_WIND : WeatherIcons.DAY_RAIN_WIND;
                    break;

                /*
                 * 1237: Ice pellets
                 * 1261: Light showers of ice pellets
                 * 1264: Moderate or heavy showers of ice pellets
                 */
                case 1237:
                case 1261:
                case 1264:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_HAIL : WeatherIcons.DAY_HAIL;
                    break;

                /* Light rain shower */
                case 1240:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS;
                    break;

                /* Patchy light rain with thunder */
                case 1273:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_STORM_SHOWERS : WeatherIcons.DAY_STORM_SHOWERS;
                    break;

                /* Moderate or heavy rain with thunder */
                case 1276:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM;
                    break;

                /*
                 * 1279: Patchy light snow with thunder
                 * 1282: Moderate or heavy snow with thunder
                 */
                case 1279:
                case 1282:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM : WeatherIcons.DAY_SNOW_THUNDERSTORM;
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