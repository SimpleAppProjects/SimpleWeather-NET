using Newtonsoft.Json.Linq;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.Web;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Http.Headers;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Extras;

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        private const String FORECAST_QUERY_URL = "https://api.met.no/weatherapi/locationforecast/2.0/complete.json?{0}";
        private const String ASTRONOMY_QUERY_URL = "https://api.met.no/weatherapi/sunrise/2.0/.json?{0}&date={1}&offset=+00:00";

        public MetnoWeatherProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.MetNo;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri forecastURL = new Uri(string.Format(FORECAST_QUERY_URL, location_query));
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                Uri sunrisesetURL = new Uri(string.Format(ASTRONOMY_QUERY_URL, location_query, date));

                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastURL))
                using (var astronomyRequest = new HttpRequestMessage(HttpMethod.Get, sunrisesetURL))
                {
                    // Add headers
                    forecastRequest.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    forecastRequest.Headers.UserAgent.AddAppUserAgent();

                    astronomyRequest.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    astronomyRequest.Headers.UserAgent.AddAppUserAgent();

                    forecastRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(15));
                    astronomyRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using (var ctsF = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    using (var forecastResponse = await webClient.SendAsync(forecastRequest, ctsF.Token))
                    {
                        await this.CheckForErrors(forecastResponse);
                        forecastResponse.EnsureSuccessStatusCode();

                        using (var ctsA = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var sunrisesetResponse = await webClient.SendAsync(astronomyRequest, ctsA.Token))
                        {
                            await this.CheckForErrors(sunrisesetResponse);
                            sunrisesetResponse.EnsureSuccessStatusCode();

                            Stream forecastStream = await forecastResponse.Content.ReadAsStreamAsync();
                            Stream sunrisesetStream = await sunrisesetResponse.Content.ReadAsStreamAsync();

                            // Reset exception
                            wEx = null;

                            // Load weather
                            Rootobject foreRoot = await JSONParser.DeserializerAsync<Rootobject>(forecastStream);
                            AstroRootobject astroRoot = await JSONParser.DeserializerAsync<AstroRootobject>(sunrisesetStream);

                            weather = new Weather(foreRoot, astroRoot);
                        }
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

                Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting weather data");
            }

            if (wEx == null && (weather == null || !weather.IsValid()))
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
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

            // The time of day is set to max if the sun never sets/rises and
            // DateTime is set to min if not found
            // Don't change this if its set that way
            if (weather.astronomy.sunrise > DateTime.MinValue &&
                weather.astronomy.sunrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.sunrise = weather.astronomy.sunrise.Add(offset);
            if (weather.astronomy.sunset > DateTime.MinValue &&
                weather.astronomy.sunset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.sunset = weather.astronomy.sunset.Add(offset);
            if (weather.astronomy.moonrise > DateTime.MinValue &&
                weather.astronomy.moonrise.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.moonrise = weather.astronomy.moonrise.Add(offset);
            if (weather.astronomy.moonset > DateTime.MinValue &&
                weather.astronomy.moonset.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                weather.astronomy.moonset = weather.astronomy.moonset.Add(offset);

            // Set condition here
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.weather = GetWeatherCondition(weather.condition.icon);
            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
                forecast.condition = GetWeatherCondition(forecast.icon);
                forecast.icon = GetWeatherIcon(forecast.icon);
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrfDate = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrfDate;

                var hrfLocalTime = hrfDate.DateTime.TimeOfDay;
                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                hr_forecast.icon = GetWeatherIcon(hrfLocalTime < sunrise || hrfLocalTime > sunset, hr_forecast.icon);
            }
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
        }

        private static String GetNeutralIconName(String icon_variant)
        {
            return icon_variant?.Replace("_day", String.Empty).Replace("_night", String.Empty).Replace("_polartwilight", String.Empty);
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        // Needed b/c icons don't show whether night or not
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null) return WeatherIcons.NA;

            //var isNeutral = icon.Split("_").Length == 1;
            var isDay = icon.EndsWith("day");
            isNight = icon.EndsWith("night");
            //var isPolarTwilight = icon.EndsWith("polartwilight");

            icon = GetNeutralIconName(icon);

            string WeatherIcon = GetNeutralIconName(icon) switch
            {
                "clearsky" => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,
                "cloudy" => WeatherIcons.CLOUDY,
                "fair" or "partlycloudy" => icon switch
                {
                    _ when isNight => WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY,
                    _ => WeatherIcons.DAY_PARTLY_CLOUDY
                },
                "fog" => WeatherIcons.FOG,
                "heavyrain" => WeatherIcons.RAIN_WIND,
                "heavyrainandthunder" => WeatherIcons.THUNDERSTORM,
                "heavyrainshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_RAIN_WIND,
                    _ when isNight => WeatherIcons.NIGHT_ALT_RAIN_WIND,
                    _ => WeatherIcons.RAIN_WIND,
                },
                "heavyrainshowersandthunder" or "lightrainshowersandthunder" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_STORM_SHOWERS,
                    _ when isNight => WeatherIcons.NIGHT_ALT_STORM_SHOWERS,
                    _ => WeatherIcons.STORM_SHOWERS,
                },
                "heavysleet" or "lightsleet" or "sleet" => WeatherIcons.SLEET,
                "heavysleetandthunder" or "lightsleetandthunder" or "sleetandthunder" => icon switch
                {
                    _ when isNight => WeatherIcons.NIGHT_ALT_SLEET_STORM,
                    _ => WeatherIcons.DAY_SLEET_STORM,
                },
                "heavysleetshowersandthunder" or "lightssleetshowersandthunder" or
                "sleetshowersandthunder" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SLEET_STORM,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SLEET_STORM,
                    _ => WeatherIcons.DAY_SLEET_STORM,
                },
                "heavysleetshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SLEET,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SLEET,
                    _ => WeatherIcons.SLEET,
                },
                "heavysnow" => WeatherIcons.SNOW_WIND,
                "heavysnowandthunder" or "heavysnowshowersandthunder" or "lightsnowandthunder" or
                "lightssnowshowersandthunder" or "snowandthunder" or "snowshowersandthunder" => icon switch
                {
                    _ when isNight => WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM,
                    _ => WeatherIcons.DAY_SNOW_THUNDERSTORM,
                },
                "heavysnowshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SNOW_WIND,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SNOW_WIND,
                    _ => WeatherIcons.SNOW_WIND,
                },
                "lightrain" => WeatherIcons.SPRINKLE,
                "lightrainandthunder" or "rainandthunder" => WeatherIcons.STORM_SHOWERS,
                "lightrainshowers" or "rainshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SHOWERS,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SHOWERS,
                    _ => WeatherIcons.SHOWERS,
                },
                "lightsleetshowers" or "sleetshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SLEET,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SLEET,
                    _ => WeatherIcons.SLEET,
                },
                "lightsnow" or "snow" => WeatherIcons.SNOW,
                "lightsnowshowers" or "snowshowers" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_SNOW,
                    _ when isNight => WeatherIcons.NIGHT_ALT_SNOW,
                    _ => WeatherIcons.SNOW,
                },
                "rain" => WeatherIcons.RAIN,
                "rainshowersandthunder" => icon switch
                {
                    _ when isDay => WeatherIcons.DAY_THUNDERSTORM,
                    _ when isNight => WeatherIcons.NIGHT_ALT_THUNDERSTORM,
                    _ => WeatherIcons.THUNDERSTORM,
                },
                _ => string.Empty
            };

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        public override String GetWeatherCondition(String icon)
        {
            if (icon == null)
                return SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_notavailable");

            var neutralIcon = GetNeutralIconName(icon);

            return neutralIcon switch
            {
                "clearsky" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_clearsky"),

                "cloudy" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_cloudy"),

                "fair" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_fair"),

                "fog" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_fog"),

                "heavyrain" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_heavyrain"),

                "heavyrainandthunder" or "heavyrainshowersandthunder" or "lightrainandthunder" or
                "lightrainshowersandthunder" or "rainandthunder" or "rainshowersandthunder" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_tstorms"),

                "heavyrainshowers" or "lightrainshowers" or "rainshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rainshowers"),

                "heavysleet" or "heavysleetshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet"),

                "heavysleetandthunder" or "heavysleetshowersandthunder" or "lightsleetandthunder" or
                "lightssleetshowersandthunder" or "sleetandthunder" or "sleetshowersandthunder" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet_tstorms"),

                "heavysnow" or "heavysnowshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_heavysnow"),

                "heavysnowandthunder" or "heavysnowshowersandthunder" or "lightsnowandthunder" or
                "lightssnowshowersandthunder" or "snowandthunder" or "snowshowersandthunder" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snow_tstorms"),

                "lightrain" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_lightrain"),

                "lightsleet" or "lightsleetshowers" or "sleet" or "sleetshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet"),

                "lightsnow" or "lightsnowshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_lightsnowshowers"),

                "partlycloudy" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_partlycloudy"),

                "rain" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rain"),

                "snow" or "snowshowers" => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snow"),

                _ => base.GetWeatherCondition(neutralIcon),
            };
        }
        
        // Met.no conditions can be for any time of day
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