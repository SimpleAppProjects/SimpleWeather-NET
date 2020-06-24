using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace SimpleWeather.WeatherYahoo
{
    public partial class YahooWeatherProvider : WeatherProviderImpl, IAstroDataProvider
    {
        public YahooWeatherProvider() : base()
        {
            LocationProvider = new Bing.BingMapsLocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.Yahoo;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => true;

        public override Task<bool> IsKeyValid(string key)
        {
            return Task.FromResult(false);
        }

        public override string GetAPIKey()
        {
            return null;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        private Task<Rootobject> GetRootobject(string location_query)
        {
            return Task.Run(async () =>
            {
                Rootobject root = null;

                string queryAPI = "https://weather-ydn-yql.media.yahoo.com/forecastrss";
                Uri weatherURL = null;

                OAuthRequest authRequest = new OAuthRequest(APIKeys.GetYahooCliID(), APIKeys.GetYahooCliSecr());

                WeatherException wEx = null;

                try
                {
                    string query = "?" + location_query + "&format=json&u=f";
                    weatherURL = new Uri(queryAPI + query);
                    string authorization = authRequest.GetAuthorizationHeader(weatherURL);

                    // Get response
                    using (HttpClient webClient = new HttpClient())
                    using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    {
                        // Add headers to request
                        request.Headers.Add("Authorization", authorization);
                        request.Headers.Add("X-Yahoo-App-Id", APIKeys.GetYahooAppID());
                        request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await webClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead).AsTask(cts.Token);
                        response.EnsureSuccessStatusCode();
                        Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
                        // Reset exception
                        wEx = null;

                        // Load weather
                        root = JSONParser.Deserializer<Rootobject>(contentStream);

                        // End Stream
                        contentStream?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    root = null;
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }

                    Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting weather data");
                }

                if (wEx != null)
                    throw wEx;

                return root;
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override Task<Weather> GetWeather(string location_query)
        {
            return Task.Run(async () =>
            {
                Weather weather = null;
                WeatherException wEx = null;

                try
                {
                    // Load weather
                    Rootobject root = await GetRootobject(location_query);

                    weather = new Weather(root);
                }
                catch (Exception ex)
                {
                    weather = null;
                    if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                    }

                    Logger.WriteLine(LoggerLevel.Error, ex, "YahooWeatherProvider: error getting weather data");
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
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<WeatherData.Astronomy> GetAstronomyData(LocationData location)
        {
            return Task.Run(async () =>
            {
                try
                {
                    String query = UpdateLocationQuery(location);
                    Rootobject root = await AsyncTask.RunAsync(GetRootobject(query));
                    return new WeatherData.Astronomy(root.current_observation.astronomy);
                }
                catch (WeatherException wEx)
                {
                    throw wEx;
                }
                catch (Exception ex)
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                }
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await AsyncTask.RunAsync(base.GetWeather(location));

            weather.update_time = weather.update_time.ToOffset(location.tz_offset);

            return weather;
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
            bool isNight = false;

            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 27: // Mostly Cloudy (Night)
                    case 29: // Partly Cloudy (Night)
                    case 31: // Clear (Night)
                    case 33: // Fair (Night)
                        isNight = true;
                        break;

                    default:
                        break;
                }
            }

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            string WeatherIcon = string.Empty;

            if (int.TryParse(icon, out int code))
            {
                switch (code)
                {
                    case 0: // Tornado
                        WeatherIcon = WeatherIcons.TORNADO;
                        break;

                    case 1: // Tropical Storm
                    case 37:
                    case 38: // Scattered Thunderstorms/showers
                    case 39:
                    case 45:
                    case 47:
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                        else
                            WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                        break;

                    case 2: // Hurricane
                        WeatherIcon = WeatherIcons.HURRICANE;
                        break;

                    case 3:
                    case 4: // Scattered Thunderstorms
                        WeatherIcon = WeatherIcons.THUNDERSTORM;
                        break;

                    case 5: // Mixed Rain/Snow
                    case 6: // Mixed Rain/Sleet
                    case 7: // Mixed Snow/Sleet
                    case 18: // Sleet
                    case 35: // Mixed Rain/Hail
                        WeatherIcon = WeatherIcons.RAIN_MIX;
                        break;

                    case 8: // Freezing Drizzle
                    case 10: // Freezing Rain
                    case 17: // Hail
                        WeatherIcon = WeatherIcons.HAIL;
                        break;

                    case 9: // Drizzle
                    case 11: // Showers
                    case 12:
                    case 40: // Scattered Showers
                        WeatherIcon = WeatherIcons.SHOWERS;
                        break;

                    case 13: // Snow Flurries
                    case 14: // Light Snow Showers
                    case 16: // Snow
                    case 42: // Scattered Snow Showers
                    case 46: // Snow Showers
                        WeatherIcon = WeatherIcons.SNOW;
                        break;

                    case 15: // Blowing Snow
                    case 41: // Heavy Snow
                    case 43:
                        WeatherIcon = WeatherIcons.SNOW_WIND;
                        break;

                    case 19: // Dust
                        WeatherIcon = WeatherIcons.DUST;
                        break;

                    case 20: // Foggy
                        WeatherIcon = WeatherIcons.FOG;
                        break;

                    case 21: // Haze
                        if (isNight)
                            WeatherIcon = WeatherIcons.WINDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_HAZE;
                        break;

                    case 22: // Smoky
                        WeatherIcon = WeatherIcons.SMOKE;
                        break;

                    case 23: // Blustery
                    case 24: // Windy
                        WeatherIcon = WeatherIcons.STRONG_WIND;
                        break;

                    case 25: // Cold
                        WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
                        break;

                    case 26: // Cloudy
                        WeatherIcon = WeatherIcons.CLOUDY;
                        break;

                    case 27: // Mostly Cloudy (Night)
                    case 28: // Mostly Cloudy (Day)
                    case 29: // Partly Cloudy (Night)
                    case 30: // Partly Cloudy (Day)
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_CLOUDY;
                        break;

                    case 31: // Clear (Night)
                    case 32: // Sunny
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY;
                        break;

                    case 33: // Fair (Night)
                    case 34: // Fair (Day)
                    case 44: // Partly Cloudy
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                        else
                            WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
                        break;

                    case 36: // HOT
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                        else
                            WeatherIcon = WeatherIcons.DAY_HOT;
                        break;

                    case 3200: // Not Available
                    default:
                        WeatherIcon = WeatherIcons.NA;
                        break;
                }
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

            switch (weather.condition.icon)
            {
                // The following cases can be present at any time of day
                case WeatherIcons.CLOUDY:
                case WeatherIcons.SNOWFLAKE_COLD:
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

                        var tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
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