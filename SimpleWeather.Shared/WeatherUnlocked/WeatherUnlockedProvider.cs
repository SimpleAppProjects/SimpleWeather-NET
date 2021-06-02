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
using Windows.Web.Http;

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
        public override async Task<WeatherData.Weather> GetWeather(string location_query, string country_code)
        {
            WeatherData.Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            try
            {
                Uri currentURL = new Uri(string.Format(CURRENT_QUERY_URL, location_query, GetAppID(), GetAppKey(), locale));
                Uri forecastURL = new Uri(string.Format(FORECAST_QUERY_URL, location_query, GetAppID(), GetAppKey(), locale));

                using (var currentRequest = new HttpRequestMessage(HttpMethod.Get, currentURL))
                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL))
                {
                    currentRequest.Headers.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
                    forecastRequest.Headers.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));

                    currentRequest.Headers.CacheControl.MaxAge = TimeSpan.FromHours(1);
                    forecastRequest.Headers.CacheControl.MaxAge = TimeSpan.FromHours(3);

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
                    using (var ctsC = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var currentResponse = await webClient.SendRequestAsync(currentRequest).AsTask(ctsC.Token))
                    using (var ctsF = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var forecastResponse = await webClient.SendRequestAsync(forecastRequest).AsTask(ctsF.Token))
                    {
                        currentResponse.EnsureSuccessStatusCode();
                        forecastResponse.EnsureSuccessStatusCode();

                        Stream currentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await currentResponse.Content.ReadAsInputStreamAsync());
                        Stream forecastStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await forecastResponse.Content.ReadAsInputStreamAsync());

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

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
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

        public override string UpdateLocationQuery(WeatherData.Weather weather)
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
            string WeatherIcon = string.Empty;

            try
            {
                int code = int.Parse(icon);

                switch (code)
                {
                    case 0: // Sunny skies/Clear skies
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY;
                        break;
                    case 1: // Partly cloudy skies
                    case 3: // Cloudy skies
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                        break;
                    case 2: // Cloudy skies
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_CLOUDY;
                        break;
                    case 10: // Haze
                    case 45: // Fog
                    case 49: // Freezing fog
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_FOG;
                        else
                            WeatherIcon = WeatherIcons.DAY_FOG;
                        break;
                    case 21: // Patchy rain possible
                    case 50: // Patchy light drizzle
                    case 51: // Light drizzle
                    case 60: // Patchy light rain
                    case 61: // Light rain
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                        else
                            WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                        break;
                    case 22: // Patchy snow possible
                    case 70: // Patchy snow possible
                    case 71: // Light snow
                    case 72: // Patchy moderate snow
                    case 73: // Moderate snow
                    case 85: // Light snow showers
                    case 86: // Moderate or heavy snow showers
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                        else
                            WeatherIcon = WeatherIcons.DAY_SNOW;
                        break;
                    case 23: // Patchy sleet possible
                    case 24: // Patchy freezing drizzle possible
                    case 56: // Freezing drizzle
                    case 57: // Heavy freezing drizzle
                    case 68: // Light sleet
                    case 69: // Moderate or heavy sleet
                    case 79: // Ice pellets
                    case 83: // Light sleet showers
                    case 84: // Moderate or heavy sleet showers
                    case 87: // Light showers of ice pellets
                    case 88: // Moderate or heavy showers of ice pellets
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET;
                        else
                            WeatherIcon = WeatherIcons.DAY_SLEET;
                        break;
                    case 29: // Thundery outbreaks possible
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_LIGHTNING;
                        else
                            WeatherIcon = WeatherIcons.DAY_LIGHTNING;
                        break;
                    case 38: // Blowing snow
                    case 39: // Blizzard
                    case 74: // Patchy heavy snow
                    case 75: // Heavy snow
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_WIND;
                        else
                            WeatherIcon = WeatherIcons.DAY_SNOW_WIND;
                        break;
                    case 62: // Moderate rain at times
                    case 63: // Moderate rain
                    case 64: // Heavy rain at times
                    case 65: // Heavy rain
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                        else
                            WeatherIcon = WeatherIcons.DAY_RAIN;
                        break;
                    case 66: // Light freezing rain
                    case 67: // Moderate or heavy freezing rain
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN_MIX;
                        else
                            WeatherIcon = WeatherIcons.DAY_RAIN_MIX;
                        break;
                    case 80: // Light rain shower
                    case 81: // Moderate or heavy rain shower
                    case 82: // Torrential rain shower
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SHOWERS;
                        else
                            WeatherIcon = WeatherIcons.DAY_SHOWERS;
                        break;
                    case 91: // Patchy light rain with thunder
                    case 92: // Moderate or heavy rain with thunder
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                        else
                            WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
                        break;
                    case 93: // Patchy light snow with thunder
                    case 94: // Moderate or heavy snow with thunder
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM;
                        else
                            WeatherIcon = WeatherIcons.DAY_SNOW_THUNDERSTORM;
                        break;
                    default:
                        WeatherIcon = WeatherIcons.NA;
                        break;
                }
            }
            catch (FormatException)
            {
                // DO nothing
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

                var tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
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