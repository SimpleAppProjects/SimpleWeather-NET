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
using SimpleWeather.Extras;

namespace SimpleWeather.WeatherUnlocked
{
    public partial class WeatherUnlockedProvider : WeatherProviderImpl
    {
        private const String BASE_URL = "http://api.weatherunlocked.com/api/";
        private const String CURRENT_QUERY_URL = BASE_URL + "current/{0}?app_id={1}&app_key={2}&lang={3}";
        private const String FORECAST_QUERY_URL = BASE_URL + "forecast/{0}?app_id={1}&app_key={2}&lang={3}";

        public WeatherUnlockedProvider() : base()
        {
            LocationProvider = new Bing.BingMapsLocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.WeatherUnlocked;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => false;

        public override int HourlyForecastInterval => 3;

        public override long GetRetryTime() => 60000;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return null;
        }

        private String GetAppID()
        {
            return APIKeys.GetWUnlockedAppID();
        }

        private String GetAppKey()
        {
            return APIKeys.GetWUnlockedKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            try
            {
                this.CheckRateLimit();

                Uri currentURL = new Uri(string.Format(CURRENT_QUERY_URL, location_query, GetAppID(), GetAppKey(), locale));
                Uri forecastURL = new Uri(string.Format(FORECAST_QUERY_URL, location_query, GetAppID(), GetAppKey(), locale));

                using (var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentURL))
                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL))
                {
                    currentRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    forecastRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    currentRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));
                    forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(1));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
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

                        weather = new Weather(currRoot, foreRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "WeatherUnlockedProvider: error getting weather data");
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

        protected override async Task UpdateWeatherData(LocationData location, Weather weather)
        {
            // OWM reports datetime in UTC; add location tz_offset
            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            weather.astronomy = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.DateTime.TimeOfDay;
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
            }
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.##},{1:0.##}", location.latitude, location.longitude);
        }

        public override String LocaleToLangCode(String iso, String name)
        {
            String code = "en";

            switch (iso)
            {
                // Danish
                case "da":
                // French
                case "fr":
                // Italian
                case "it":
                // German
                case "de":
                // Dutch
                case "nl":
                // Spanish
                case "es":
                // Norwegian
                case "no":
                // Swedish
                case "sv":
                // Turkish
                case "tr":
                // Bulgarian
                case "bg":
                // Czech
                case "cs":
                // Hungarian
                case "hu":
                // Polish
                case "pl":
                // Russian
                case "ru":
                // Slovak
                case "sk":
                    code = iso;
                    break;
                // Romanian
                case "ro":
                    code = "rm";
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
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (!int.TryParse(icon, out int code))
            {
                return WeatherIcons.NA;
            }

            string WeatherIcon = code switch
            {
                0 /* Sunny skies/Clear skies */ 
                => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                1 /* Partly cloudy skies */ 
                => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,

                2 /* Cloudy skies */
                => WeatherIcons.CLOUDY,

                3 /* Overcast skies */
                => WeatherIcons.OVERCAST,

                10 or // Mist
                45 or // Fog
                49 // Freezing fog
                => WeatherIcons.FOG,

                21 or // Patchy rain possible
                50 or // Patchy light drizzle
                60 // Patchy light rain
                => isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE,

                22 or // Patchy snow possible
                70 or // Patchy light snow
                72 // Patchy moderate snow
                => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,

                23 // Patchy sleet possible
                => isNight ? WeatherIcons.NIGHT_ALT_SLEET : WeatherIcons.DAY_SLEET,

                24 // Patchy freezing drizzle possible
                => isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX,

                29 /* Thundery outbreaks possible */
                => isNight ? WeatherIcons.NIGHT_ALT_LIGHTNING : WeatherIcons.DAY_LIGHTNING,
                
                38 or // Blowing snow
                39 or // Blizzard
                74 or // Patchy heavy snow
                75 // Heavy snow
                => WeatherIcons.SNOW_WIND,

                51 or // Light drizzle
                61 // Light rain
                => WeatherIcons.SPRINKLE,

                56 or // Freezing drizzle
                57 or // Heavy freezing drizzle
                66 or // Light freezing rain
                67 // Moderate or heavy freezing rain
                => WeatherIcons.RAIN_MIX,

                62 or // Moderate rain at times
                63 /* Moderate rain */
                => WeatherIcons.RAIN,

                64 or // Heavy rain at times
                65 // Heavy rain
                => WeatherIcons.RAIN_WIND,

                68 or // Light sleet
                69 // Moderate or heavy sleet
                => WeatherIcons.SLEET,

                71 or // Light snow
                73 // Moderate snow
                => WeatherIcons.SNOW,

                79 // Ice pellets
                => WeatherIcons.HAIL,

                80 or // Light rain shower
                81 or // Moderate or heavy rain shower
                82 // Torrential rain shower
                => isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS,

                83 or // Light sleet showers
                84 // Moderate or heavy sleet showers
                => isNight ? WeatherIcons.NIGHT_ALT_SLEET : WeatherIcons.DAY_SLEET,

                85 or // Light snow showers
                86 // Moderate or heavy snow showers
                => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,

                87 or // Light showers of ice pellets
                88 /* Moderate or heavy showers of ice pellets */
                => isNight ? WeatherIcons.NIGHT_ALT_HAIL : WeatherIcons.DAY_HAIL,

                91 // Patchy light rain with thunder
                => isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM,

                92 /* Moderate or heavy rain with thunder */
                => WeatherIcons.THUNDERSTORM,

                93 // Patchy light snow with thunder
                => isNight ? WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM : WeatherIcons.DAY_SNOW_THUNDERSTORM,

                94 /* Moderate or heavy snow with thunder */
                => WeatherIcons.SNOW_THUNDERSTORM,

                _ => WeatherIcons.NA,
            };

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