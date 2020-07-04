using Newtonsoft.Json.Linq;
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
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace SimpleWeather.Metno
{
    public partial class MetnoWeatherProvider : WeatherProviderImpl
    {
        public MetnoWeatherProvider() : base()
        {
            LocationProvider = new Bing.BingMapsLocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.MetNo;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => true;

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override String GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<Weather> GetWeather(string location_query)
        {
            return Task.Run(async () =>
            {
                Weather weather = null;

                string forecastAPI = null;
                Uri forecastURL = null;
                string sunrisesetAPI = null;
                Uri sunrisesetURL = null;

                forecastAPI = "https://api.met.no/weatherapi/locationforecast/2.0/complete.json?{0}";
                forecastURL = new Uri(string.Format(forecastAPI, location_query));
                sunrisesetAPI = "https://api.met.no/weatherapi/sunrise/2.0/.json?{0}&date={1}&offset=+00:00";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                sunrisesetURL = new Uri(string.Format(sunrisesetAPI, location_query, date));

                using (var handler = new HttpBaseProtocolFilter()
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = true
                })
                using (HttpClient webClient = new HttpClient(handler))
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                {
                    // Use GZIP compression
                    var version = string.Format("v{0}.{1}.{2}",
                        Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                    webClient.DefaultRequestHeaders.AcceptEncoding.Add(new HttpContentCodingWithQualityHeaderValue("gzip"));
                    webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                    WeatherException wEx = null;

                    try
                    {
                        // Get response
                        HttpResponseMessage forecastResponse = await webClient.GetAsync(forecastURL).AsTask(cts.Token);
                        forecastResponse.EnsureSuccessStatusCode();
                        HttpResponseMessage sunrisesetResponse = await webClient.GetAsync(sunrisesetURL).AsTask(cts.Token);
                        sunrisesetResponse.EnsureSuccessStatusCode();

                        Stream forecastStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await forecastResponse.Content.ReadAsInputStreamAsync());
                        Stream sunrisesetStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await sunrisesetResponse.Content.ReadAsInputStreamAsync());

                        // Reset exception
                        wEx = null;

                        // Load weather
                        Rootobject foreRoot = JSONParser.Deserializer<Rootobject>(forecastStream);
                        AstroRootobject astroRoot = JSONParser.Deserializer<AstroRootobject>(sunrisesetStream);

                        weather = new Weather(foreRoot, astroRoot);
                        // Release resources
                        cts.Dispose();
                    }
                    catch (Exception ex)
                    {
                        weather = null;

                        if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                        {
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                        }

                        Logger.WriteLine(LoggerLevel.Error, ex, "MetnoWeatherProvider: error getting weather data");
                    }

                    // End Stream
                    webClient.Dispose();
                    handler.Dispose();

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
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await AsyncTask.RunAsync(base.GetWeather(location));

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
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.LocalDateTime.TimeOfDay;
                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
            }

            return weather;
        }

        private static string GetWeatherCondition(string icon)
        {
            var icon_neutral = GetNeutralIconName(icon);

            var icon_obj = LegendObject.GetValue(icon_neutral);

            if (icon_obj != null)
            {
                var condition = icon_obj.Value<String>("desc_en");
                return condition;
            }

            return Weather.NA;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format("lat={0}&lon={1}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format("lat={0}&lon={1}", location.latitude.ToInvariantString("0.####"), location.longitude.ToInvariantString("0.####"));
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        // Needed b/c icons don't show whether night or not
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;
            icon = GetNeutralIconName(icon);

            switch (icon)
            {
                case "clearsky":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                    break;

                case "fair":
                case "partlycloudy":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                case "cloudy":
                    WeatherIcon = WeatherIcons.CLOUDY;
                    break;

                case "rainshowers":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "rainshowersandthunder":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
                    break;

                case "sleetshowers":
                case "lightsleetshowers":
                case "heavysleetshowers":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET;
                    break;

                case "snowshowers":
                case "lightsnowshowers":
                case "heavysnowshowers":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW;
                    break;

                case "rain":
                case "lightrain":
                    WeatherIcon = WeatherIcons.SPRINKLE;
                    break;

                case "heavyrain":
                    WeatherIcon = WeatherIcons.RAIN;
                    break;

                case "heavyrainandthunder":
                    WeatherIcon = WeatherIcons.THUNDERSTORM;
                    break;

                case "sleet":
                case "lightsleet":
                case "heavysleet":
                    WeatherIcon = WeatherIcons.SLEET;
                    break;

                case "snow":
                case "lightsnow":
                    WeatherIcon = WeatherIcons.SNOW;
                    break;

                case "snowandthunder":
                case "snowshowersandthunder":
                case "lightssnowshowersandthunder":
                case "heavysnowshowersandthunder":
                case "lightsnowandthunder":
                case "heavysnowandthunder":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW_THUNDERSTORM;
                    break;

                case "fog":
                    WeatherIcon = WeatherIcons.FOG;
                    break;

                case "sleetshowersandthunder":
                case "sleetandthunder":
                case "lightssleetshowersandthunder":
                case "heavysleetshowersandthunder":
                case "lightsleetandthunder":
                case "heavysleetandthunder":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET_STORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET_STORM;
                    break;

                case "rainandthunder":
                case "lightrainandthunder":
                case "lightrainshowersandthunder":
                case "heavyrainshowersandthunder":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                    else
                        WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                    break;

                case "lightrainshowers":
                case "heavyrainshowers":
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "heavysnow":
                    WeatherIcon = WeatherIcons.SNOW_WIND;
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