using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.OpenWeather.OneCall
{
    public partial class OWMOneCallWeatherProvider : WeatherProviderImpl, IAirQualityProvider
    {
        private const String BASE_URL = "https://api.openweathermap.org/data/2.5/";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "forecast?appid={0}";
        private const String WEATHER_QUERY_URL = BASE_URL + "onecall?{0}&appid={1}&lang={2}";
        private const String AQI_QUERY_URL = BASE_URL + "air_pollution?lat={0:0.####}&lon={1:0.####}&appid={2}";

        public OWMOneCallWeatherProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new Bing.BingMapsLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.OpenWeatherMap;
        public override bool SupportsWeatherLocale => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => true;

        public override int HourlyForecastInterval => 1;
        public override AuthType AuthType => AuthType.ApiKey;

        public override long GetRetryTime() => 43200000L; // 12 hrs

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                if (String.IsNullOrWhiteSpace(key))
                    throw (wEx = new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey));

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

            if (wEx != null)
            {
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetOWMKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<SimpleWeather.WeatherData.Weather> GetWeather(string location_query, string country_code)
        {
            SimpleWeather.WeatherData.Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;

            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            string query;
            if (int.TryParse(location_query, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            {
                query = string.Format("id={0}", id);
            }
            else
            {
                query = location_query;
            }

            var key = SettingsManager.UsePersonalKey ? SettingsManager.APIKeys[WeatherAPI] : GetAPIKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            try
            {
                this.CheckRateLimit();

                Uri weatherURL = new Uri(string.Format(WEATHER_QUERY_URL, query, key, locale));

                using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(stream);

                        weather = this.CreateWeatherData(root);
                    }
                }
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

                Logger.WriteLine(LoggerLevel.Error, ex, "OWMOneCallWeatherProvider: error getting weather data");
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

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location, SimpleWeather.WeatherData.Weather weather)
        {
            // OWM reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);
            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date = hr_forecast.date.ToOffset(offset);
            }
            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }
            if (weather.min_forecast?.Any() == true)
            {
                foreach (MinutelyForecast min_forecast in weather.min_forecast)
                {
                    min_forecast.date = min_forecast.date.ToOffset(offset);
                }
            }

            weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
            weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);

            if (weather.astronomy.moonrise == DateTime.MinValue || weather.astronomy.moonset == DateTime.MinValue)
            {
                var old = weather.astronomy;
                var newAstro = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
                newAstro.sunrise = old.sunrise;
                newAstro.sunset = old.sunset;
                weather.astronomy = newAstro;
            }
            else
            {
                weather.astronomy.moonrise = weather.astronomy.moonrise.Add(offset);
                weather.astronomy.moonset = weather.astronomy.moonset.Add(offset);
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (WeatherAlert alert in weather.weather_alerts)
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

        public async Task<AirQualityData> GetAirQualityData(SimpleWeather.LocationData.LocationData location)
        {
            AirQualityData aqiData = null;

            var key = SettingsManager.UsePersonalKey ? SettingsManager.APIKeys[WAPI.OpenWeatherMap] : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Uri weatherURL = new Uri(string.Format(AQI_QUERY_URL, location.latitude, location.longitude, key));

                using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
                    using (var response = await webClient.SendAsync(request, cts.Token))
                    {
                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        Stream stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        var root = await JSONParser.DeserializerAsync<AirPollutionRootobject>(stream);

                        aqiData = this.CreateAirQuality(root);
                    }
                }
            }
            catch (Exception ex)
            {
                aqiData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "OWMOneCallWeatherProvider: error getting aqi data");
            }

            return aqiData;
        }

        public override string UpdateLocationQuery(SimpleWeather.WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            string code = iso switch
            {
                // Arabic
                "af" or // Afrikaans
                "ar" or // Arabic
                "az" or // Azerbaijani
                "bg" or // Bulgarian
                "ca" or // Catalan
                "da" or // Danish
                "de" or // German
                "el" or // Greek
                "eu" or // Basque
                "fa" or // Persian (Farsi)
                "fi" or // Finnish
                "fr" or // French
                "gl" or // Galician
                "he" or // Hebrew
                "hi" or // Hindi
                "hr" or // Croatian
                "hu" or // Hungarian
                "id" or // Indonesian
                "it" or // Italian
                "ja" or // Japanese
                "lt" or // Lithuanian
                "mk" or // Macedonian
                "no" or // Norwegian
                "nl" or // Dutch
                "pl" or // Polish
                "ro" or // Romanian
                "ru" or // Russian
                "sv" or // Swedish (sv or se)
                "sk" or // Slovak
                "sl" or // Slovenian
                "es" or // Spanish
                "sr" or // Serbian
                "th" or // Thai
                "tr" or // Turkish
                "uk" or // Ukrainian
                "vi" or // Vietnamese
                "zu" /* Zulu */ => iso,
                // Chinese
                "zh" => name switch
                {
                    // Chinese - Traditional
                    "zh-Hant" or
                    "zh-HK" or
                    "zh-MO" or
                    "zh-TW" => "zh_tw",
                    // Chinese - Simplified
                    _ => "zh_cn",
                },
                // Portuguese
                "pt" => name switch
                {
                    // Português Brasil
                    "pt-BR" => "pt_br",
                    _ => "pt"
                },
                // Albanian
                "sq" => "al",
                // Czech
                "cs" => "cz",
                // Korean
                "ko" => "kr",
                // Latvian
                "lv" => "la",
                _ => "en" // Default is English
            };
            return code;
        }

        public override string GetWeatherIcon(string icon)
        {
            bool isNight = false;

            if (icon == null)
                return WeatherIcons.NA;

            if (icon.EndsWith("n"))
                isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null)
                return WeatherIcons.NA;

            string WeatherIcon = string.Empty;

            switch (icon.Substring(0, 3))
            {
                case "200": // thunderstorm w/ light rain
                case "201": // thunderstorm w/ rain
                case "210": // light thunderstorm
                case "230": // thunderstorm w/ light drizzle
                case "231": // thunderstorm w/ drizzle
                    WeatherIcon = WeatherIcons.STORM_SHOWERS;
                    break;

                case "211": // thunderstorm
                case "212": // heavy thunderstorm
                case "221": // ragged thunderstorm
                case "202": // thunderstorm w/ heavy rain
                case "232": // thunderstorm w/ heavy drizzle
                    WeatherIcon = WeatherIcons.THUNDERSTORM;
                    break;

                case "300": // light intensity drizzle
                case "301": // drizzle
                case "310": // light intensity drizzle rain
                case "321": // shower drizzle
                    WeatherIcon = WeatherIcons.SPRINKLE;
                    break;

                case "302": // heavy intensity drizzle
                case "311": // drizzle rain
                case "312": // heavy intensity drizzle rain
                case "314": // heavy shower rain and drizzle
                    WeatherIcon = WeatherIcons.RAIN;
                    break;

                case "500": // light rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "501": // moderate rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "502": // heavy intensity rain
                case "503": // very heavy rain
                case "504": // extreme rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN_WIND;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN_WIND;
                    break;

                case "511": // freezing rain
                case "611": // sleet
                case "612": // light shower sleet
                case "613": // shower sleet
                case "615": // light rain and snow
                case "616": // rain and snow
                case "620": // light shower snow
                    WeatherIcon = WeatherIcons.RAIN_MIX;
                    break;

                case "313": // shower rain and drizzle
                case "520": // light intensity shower rain
                case "521": // shower rain
                case "522": // heavy intensity shower rain
                case "531": // ragged shower rain
                    WeatherIcon = WeatherIcons.SHOWERS;
                    break;

                case "600": // light snow
                case "601": // snow
                case "621": // shower snow
                    WeatherIcon = WeatherIcons.SNOW;
                    break;

                case "602": // heavy snow
                case "622": // heavy shower snow
                    WeatherIcon = WeatherIcons.SNOW_WIND;
                    break;

                // smoke
                case "711":
                    WeatherIcon = WeatherIcons.SMOKE;
                    break;

                // haze
                case "721":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_HAZE;
                    else
                        WeatherIcon = WeatherIcons.DAY_HAZE;
                    break;

                case "731": // sand/ dust whirls
                case "761": /* dust */
                    WeatherIcon = WeatherIcons.DUST;
                    break;

                case "701": // mist
                case "741": // fog
                    WeatherIcon = WeatherIcons.FOG;
                    break;

                // Sand
                case "751":
                    WeatherIcon = WeatherIcons.SANDSTORM;
                    break;

                // volcanic ash
                case "762":
                    WeatherIcon = WeatherIcons.VOLCANO;
                    break;

                // squalls
                case "771":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY_GUSTS;
                    break;

                // tornado
                case "781":
                case "900":
                    WeatherIcon = WeatherIcons.TORNADO;
                    break;

                case "800": // clear sky
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                    break;

                case "801": // few clouds
                case "802": // scattered clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_PARTLY_CLOUDY;
                    break;

                case "803": // broken clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY;
                    break;

                case "804": // overcast clouds
                    WeatherIcon = WeatherIcons.OVERCAST;
                    break;

                case "901": // tropical storm
                case "902": // hurricane
                    WeatherIcon = WeatherIcons.HURRICANE;
                    break;

                // cold
                case "903":
                    WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
                    break;

                // hot
                case "904":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_HOT;
                    else
                        WeatherIcon = WeatherIcons.DAY_HOT;
                    break;

                // windy
                case "905":
                    WeatherIcon = WeatherIcons.WINDY;
                    break;

                // hail
                case "906":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_HAIL;
                    else
                        WeatherIcon = WeatherIcons.DAY_HAIL;
                    break;

                // strong wind
                case "957":
                    WeatherIcon = WeatherIcons.STRONG_WIND;
                    break;
            }

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
        public override bool IsNight(SimpleWeather.WeatherData.Weather weather)
        {
            bool isNight = base.IsNight(weather);

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.THUNDERSTORM:
                case WeatherIcons.SPRINKLE:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_WIND:
                case WeatherIcons.SMOKE:
                case WeatherIcons.DUST:
                case WeatherIcons.FOG:
                case WeatherIcons.SANDSTORM:
                case WeatherIcons.VOLCANO:
                case WeatherIcons.TORNADO:
                case WeatherIcons.OVERCAST:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.WINDY:
                case WeatherIcons.STRONG_WIND:
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
                    break;

                default:
                    break;
            }

            return isNight;
        }
    }
}