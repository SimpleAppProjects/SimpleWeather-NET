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
        private const String QUERY_URL = "https://weather-ydn-yql.media.yahoo.com/forecastrss?{0}&format=json&u=f";
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
                WeatherException wEx = null;

                try
                {
                    Uri weatherURL = new Uri(String.Format(QUERY_URL, location_query));
                    OAuthRequest authRequest = new OAuthRequest(APIKeys.GetYahooCliID(), APIKeys.GetYahooCliSecr());
                    string authorization = authRequest.GetAuthorizationHeader(weatherURL);

                    using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                    {
                        // Add headers to request
                        request.Headers.Add("Authorization", authorization);
                        request.Headers.Add("X-Yahoo-App-Id", APIKeys.GetYahooAppID());
                        request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                        // Get response
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                        using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                        {
                            response.EnsureSuccessStatusCode();
                            Stream contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                            // Load weather
                            root = JSONParser.Deserializer<Rootobject>(contentStream);
                        }
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
        public override Task<Weather> GetWeather(string location_query, string country_code)
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
                    Rootobject root = await GetRootobject(query);
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
            var weather = await base.GetWeather(location);

            weather.update_time = weather.update_time.ToOffset(location.tz_offset);

            return weather;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "lat={0:0.####}&lon={1:0.####}", location.latitude, location.longitude);
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

                    case 37: // isolated thunderstorms
                    case 38: // Scattered Thunderstorms/showers
                    case 39: // scattered showers (day)
                    case 45: // scattered showers (night)
                    case 47: // scattered thundershowers
                        if (isNight)
                            WeatherIcon = WeatherIcons.NIGHT_ALT_STORM_SHOWERS;
                        else
                            WeatherIcon = WeatherIcons.DAY_STORM_SHOWERS;
                        break;

                    case 1: // Tropical Storm
                    case 2: // Hurricane
                        WeatherIcon = WeatherIcons.HURRICANE;
                        break;

                    case 3: // severe thunderstorms
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
                    case 12: // rain
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
                    case 43: // blizzard
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

                    case 29: // Partly Cloudy (Night)
                    case 30: // Partly Cloudy (Day)
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

        public override String GetWeatherCondition(String icon)
        {
            try
            {
                int code = int.Parse(icon);

                switch (code)
                {
                    case 0: // Tornado
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tornado");
                    case 37: // isolated thunderstorms
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_isotstorms");
                    case 39: // scattered showers (day)
                    case 45: // scattered showers (night)
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_scatteredshowers");
                    case 38: // Scattered Thunderstorms/showers
                    case 47: // scattered thundershowers
                    case 4: // Scattered Thunderstorms
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_scatteredtstorms");
                    case 1: // Tropical Storm
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tropicalstorm");
                    case 2: // Hurricane
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hurricane");
                    case 3: // severe thunderstorms
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_severetstorms");
                    case 5: // Mixed Rain/Snow
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandsnow");
                    case 6: // Mixed Rain/Sleet
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandsleet");
                    case 7: // Mixed Snow/Sleet
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snowandsleet");
                    case 18: // Sleet
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sleet");
                    case 17: // Hail
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hail");
                    case 35: // Mixed Rain/Hail
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandhail");
                    case 8: // Freezing Drizzle
                    case 10: // Freezing Rain
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_freezingrain");
                    case 9: // Drizzle
                    case 12: // rain
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rain");
                    case 11: // Showers
                    case 40: // Scattered Showers
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainshowers");
                    case 13: // Snow Flurries
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snowflurries");
                    case 16: // Snow
                    case 46: // Snow Showers
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snow");
                    case 14: // Light Snow Showers
                    case 42: // Scattered Snow Showers
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_lightsnowshowers");
                    case 15: // Blowing Snow
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_blowingsnow");
                    case 41: // Heavy Snow
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_heavysnow");
                    case 43: // blizzard
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_blizzard");
                    case 19: // Dust
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_dust");
                    case 20: // Foggy
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_foggy");
                    case 21: // Haze
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_haze");
                    case 22: // Smoky
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_smoky");
                    case 23: // Blustery
                    case 24: // Windy
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_windy");
                    case 25: // Cold
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cold");
                    case 26: // Cloudy
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cloudy");
                    case 27: // Mostly Cloudy (Night)
                    case 28: // Mostly Cloudy (Day)
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_mostlycloudy");
                    case 29: // Partly Cloudy (Night)
                    case 30: // Partly Cloudy (Day)
                    case 44: // Partly Cloudy
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_partlycloudy");
                    case 31: // Clear (Night)
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_clear");
                    case 32: // Sunny
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sunny");
                    case 33: // Fair (Night)
                    case 34: // Fair (Day)
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_fair");
                    case 36: // HOT
                        return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hot");
                }
            }
            catch (FormatException)
            {
            }

            return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_notavailable");
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