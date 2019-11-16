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
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(false);
            return tcs.Task;
        }

        public override String GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query)
        {
            Weather weather = null;

            string forecastAPI = null;
            Uri forecastURL = null;
            string sunrisesetAPI = null;
            Uri sunrisesetURL = null;

            forecastAPI = "https://api.met.no/weatherapi/locationforecastlts/1.3/?{0}";
            forecastURL = new Uri(string.Format(forecastAPI, location_query));
            sunrisesetAPI = "https://api.met.no/weatherapi/sunrise/2.0/?{0}&date={1}&offset=+00:00";
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
                    weatherdata foreRoot = null;
                    astrodata astroRoot = null;

                    XmlSerializer foreDeserializer = new XmlSerializer(typeof(weatherdata));
                    foreRoot = (weatherdata)foreDeserializer.Deserialize(forecastStream);
                    XmlSerializer astroDeserializer = new XmlSerializer(typeof(astrodata));
                    astroRoot = (astrodata)astroDeserializer.Deserialize(sunrisesetStream);

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

                    foreach (Forecast forecast in weather.forecast)
                    {
                        forecast.condition = GetWeatherCondition(forecast.icon);
                        forecast.icon = GetWeatherIcon(forecast.icon);
                    }
                }

                if (wEx != null)
                    throw wEx;

                return weather;
            }
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

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

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.Add(offset);
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                hr_forecast.date = hr_forecast.date.ToOffset(offset);

                var hrnow = hr_forecast.date.TimeOfDay;
                var sunriseTime = weather.astronomy.sunrise.TimeOfDay;
                var sunsetTime = weather.astronomy.sunset.TimeOfDay;

                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
                hr_forecast.icon = GetWeatherIcon(hrnow < sunriseTime || hrnow > sunsetTime, hr_forecast.icon);
            }

            return weather;
        }

        // TODO: Move this out
        public static string GetWeatherCondition(string icon)
        {
            string condition = String.Empty;

            switch (icon)
            {
                case "1": // Sun
                    condition = "Clear";
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    condition = "Partly Cloudy";
                    break;

                case "4": // Cloud
                    condition = "Mostly Cloudy";
                    break;

                case "5": // LightRainSun
                case "6": // LightRainThunderSun
                case "9": // LightRain
                case "22": // LightRainThunder
                    condition = "Light Rain";
                    break;

                case "7": // SleetSun
                case "12": // Sleet
                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                case "47": // LightSleet
                case "48": // HeavySleet
                    condition = "Sleet";
                    break;

                case "8": // SnowSun
                case "13": // Snow
                case "14": // SnowThunder
                case "21": // SnowSunThunder
                    condition = "Snow";
                    break;

                case "10": // Rain
                case "11": // RainThunder
                case "25": // RainThunderSun
                case "41": // RainSun
                    condition = "Rain";
                    break;

                case "15": // Fog
                    condition = "Fog";
                    break;

                case "24": // DrizzleThunderSun
                case "30": // DrizzleThunder
                case "40": // DrizzleSun
                case "46": // Drizzle
                    condition = "Drizzle";
                    break;

                case "28": // LightSnowThunderSun
                case "33": // LightSnowThunder
                case "44": // LightSnowSun
                case "49": // LightSnow
                    condition = "Light Snow";
                    break;

                case "29": // HeavySnowThunderSun
                case "34": // HeavySnowThunder
                case "45": // HeavySnowSun
                case "50": // HeavySnow
                    condition = "Heavy Snow";
                    break;

                default:
                    condition = Weather.NA;
                    break;
            }

            return condition;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format("lat={0}&lon={1}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format("lat={0}&lon={1}", location.latitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        // Needed b/c icons don't show whether night or not
        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            switch (icon)
            {
                case "1": // Sun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY;
                    break;

                case "2": // LightCloud
                case "3": // PartlyCloud
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                    else
                        WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                case "4": // Cloud
                    WeatherIcon = WeatherIcons.CLOUDY;
                    break;

                case "5": // LightRainSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SPRINKLE;
                    else
                        WeatherIcon = WeatherIcons.DAY_SPRINKLE;
                    break;

                case "6": // LightRainThunderSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
                    break;

                case "7": // SleetSun
                case "42": // LightSleetSun
                case "43": // HeavySleetSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET;
                    break;

                case "8": // SnowSun
                case "44": // LightSnowSun
                case "45": // HeavySnowSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW;
                    break;

                case "9": // LightRain
                case "46": // Drizzle
                    WeatherIcon = WeatherIcons.SPRINKLE;
                    break;

                case "10": // Rain
                    WeatherIcon = WeatherIcons.RAIN;
                    break;

                case "11": // RainThunder
                    WeatherIcon = WeatherIcons.THUNDERSTORM;
                    break;

                case "12": // Sleet
                case "47": // LightSleet
                case "48": // HeavySleet
                    WeatherIcon = WeatherIcons.SLEET;
                    break;

                case "13": // Snow
                case "49": // LightSnow
                    WeatherIcon = WeatherIcons.SNOW;
                    break;

                case "14": // SnowThunder
                case "21": // SnowSunThunder
                case "28": // LightSnowThunderSun
                case "29": // HeavySnowThunderSun
                case "33": // LightSnowThunder
                case "34": // HeavySnowThunder
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SNOW_THUNDERSTORM;
                    break;

                case "15": // Fog
                    WeatherIcon = WeatherIcons.FOG;
                    break;

                case "20": // SleetSunThunder
                case "23": // SleetThunder
                case "26": // LightSleetThunderSun
                case "27": // HeavySleetThunderSun
                case "31": // LightSleetThunder
                case "32": // HeavySleetThunder
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET_STORM;
                    else
                        WeatherIcon = WeatherIcons.DAY_SLEET_STORM;
                    break;

                case "22": // LightRainThunder
                case "30": // DrizzleThunder
                case "24": // DrizzleThunderSun
                case "25": // RainThunderSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                    else
                        WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                    break;

                case "40": // DrizzleSun
                case "41": // RainSun
                    if (isNight)
                        WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                    else
                        WeatherIcon = WeatherIcons.DAY_RAIN;
                    break;

                case "50": // HeavySnow
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