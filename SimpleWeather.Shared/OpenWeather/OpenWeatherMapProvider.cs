using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
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

namespace SimpleWeather.OpenWeather
{
    public partial class OpenWeatherMapProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "https://api.openweathermap.org/data/2.5/";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "forecast?appid={0}";
        private const String CURRENT_QUERY_URL = BASE_URL + "weather?{0}&appid={1}&lang={2}";
        private const String FORECAST_QUERY_URL = BASE_URL + "forecast?{0}&appid={1}&lang={2}";

        public OpenWeatherMapProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.OpenWeatherMap;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;

        public override int HourlyForecastInterval => 3;

        public override long GetRetryTime() => 60000;

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
        public override async Task<WeatherData.Weather> GetWeather(string location_query, string country_code)
        {
            WeatherData.Weather weather = null;
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

            string key = Settings.UsePersonalKey ? Settings.API_KEY : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Uri currentURL = new Uri(string.Format(CURRENT_QUERY_URL, query, key, locale));
                Uri forecastURL = new Uri(string.Format(FORECAST_QUERY_URL, query, key, locale));

                using (var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentURL))
                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL))
                {
                    currentRequest.Headers.CacheControl = new CacheControlHeaderValue() 
                    {
                        MaxAge = TimeSpan.FromHours(1)
                    };
                    forecastRequest.Headers.CacheControl = new CacheControlHeaderValue() 
                    {
                        MaxAge = TimeSpan.FromHours(3)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var ctsC = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var currentResponse = await webClient.SendAsync(currentRequest, ctsC.Token))
                    using (var ctsF = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token))
                    {
                        await this.CheckForErrors(currentResponse);
                        currentResponse.EnsureSuccessStatusCode();

                        await this.CheckForErrors(forecastResponse);
                        forecastResponse.EnsureSuccessStatusCode();

                        Stream currentStream = await currentResponse.Content.ReadAsStreamAsync();
                        Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();

                        // Load weather
                        CurrentRootobject currRoot = await JSONParser.DeserializerAsync<CurrentRootobject>(currentStream);
                        ForecastRootobject foreRoot = await JSONParser.DeserializerAsync<ForecastRootobject>(forecastStream);

                        weather = new WeatherData.Weather(currRoot, foreRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "OpenWeatherMapProvider: error getting weather data");
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
            weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
            weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);

            var old = weather.astronomy;
            var newAstro = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            newAstro.sunrise = old.sunrise;
            newAstro.sunset = old.sunset;
            weather.astronomy = newAstro;
        }

        public override string UpdateLocationQuery(WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            string code = "en";

            switch (iso)
            {
                // Arabic
                case "ar":
                // Bulgarian
                case "bg":
                // Catalan
                case "ca":
                // Croatian
                case "hr":
                // Dutch
                case "nl":
                // Farsi / Persian
                case "fa":
                // Finnish
                case "fi":
                // French
                case "fr":
                // Galician
                case "gl":
                // German
                case "de":
                // Greek
                case "el":
                // Hungarian
                case "hu":
                // Italian
                case "it":
                // Japanese
                case "ja":
                // Lithuanian
                case "lt":
                // Macedonian
                case "mk":
                // Polish
                case "pl":
                // Portuguese
                case "pt":
                // Romanian
                case "ro":
                // Russian
                case "ru":
                // Slovak
                case "sk":
                // Slovenian
                case "sl":
                // Spanish
                case "es":
                // Turkish
                case "tr":
                // Vietnamese
                case "vi":
                    code = iso;
                    break;
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
                        // Chinese - Simplified
                        default:
                            code = "zh_cn";
                            break;
                    }
                    break;
                // Czech
                case "cs":
                    code = "cz";
                    break;
                // Korean
                case "ko":
                    code = "kr";
                    break;
                // Latvian
                case "lv":
                    code = "la";
                    break;
                // Swedish
                case "sv":
                    code = "se";
                    break;
                // Ukrainian
                case "uk":
                    code = "ua";
                    break;

                default:
                    // Default is English
                    code = "en";
                    break;
            }

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
            string WeatherIcon = string.Empty;

            if (icon == null)
                return WeatherIcons.NA;

            switch (icon.Substring(0, 3))
            {
                case "200": // thunderstorm w/ light rain
                case "201": // thunderstorm w/ rain
                case "210": // light thunderstorm
                case "230": // thunderstorm w/ light drizzle
                case "231": // thunderstorm w/ drizzle
                case "531": // ragged shower rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                    else
                        WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                    break;

                case "211": // thunderstorm
                case "212": // heavy thunderstorm
                case "221": // ragged thunderstorm
                case "202": // thunderstorm w/ heavy rain
                case "232": // thunderstorm w/ heavy drizzle
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
                    break;

                case "300": // light intensity drizzle
                case "301": // drizzle
                case "321": // shower drizzle
                case "500": // light rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "302": // heavy intensity drizzle
                case "311": // drizzle rain
                case "312": // heavy intensity drizzle rain
                case "314": // heavy shower rain and drizzle
                case "501": // moderate rain
                case "502": // heavy intensity rain
                case "503": // very heavy rain
                case "504": // extreme rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "310": // light intensity drizzle rain
                case "511": // freezing rain
                case "611": // sleet
                case "612": // shower sleet
                case "615": // light rain and snow
                case "616": // rain and snow
                case "620": // light shower snow
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN_MIX;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN_MIX;
                    break;

                case "313": // shower rain and drizzle
                case "520": // light intensity shower rain
                case "521": // shower rain
                case "522": // heavy intensity shower rain
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SHOWERS;
                    else
                        WeatherIcon = WeatherIcons.DAY_SHOWERS;
                    break;

                case "600": // light snow
                case "601": // snow
                case "621": // shower snow
                case "622": // heavy shower snow
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW;
                    break;

                case "602": // heavy snow
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_WIND;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW_WIND;
                    break;

                // smoke
                case "711":
                    WeatherIcon = WeatherIcons.SMOKE;
                    break;

                // haze
                case "721":
                    if (isNight)
                        WeatherIcon = WeatherIcons.WINDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_HAZE;
                    break;

                // dust
                case "731":
                case "761":
                case "762":
                    WeatherIcon = WeatherIcons.DUST;
                    break;

                case "741": // fog
                case "701": // mist
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_FOG;
                    else
                        WeatherIcon = WeatherIcons.DAY_FOG;
                    break;

                // cloudy-gusts
                case "771": // squalls
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

                // day-sunny
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
                        WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                case "803": // broken clouds
                case "804": // overcast clouds
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_CLOUDY;
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
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
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

                default:
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
        public override bool IsNight(WeatherData.Weather weather)
        {
            bool isNight = base.IsNight(weather);

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.SMOKE:
                case WeatherIcons.WINDY:
                case WeatherIcons.DUST:
                case WeatherIcons.TORNADO:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.HAIL:
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