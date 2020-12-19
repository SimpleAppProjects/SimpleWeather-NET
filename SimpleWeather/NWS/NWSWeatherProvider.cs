using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Web;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace SimpleWeather.NWS
{
    public partial class NWSWeatherProvider : WeatherProviderImpl
    {
        private const string POINTS_QUERY_URL = "https://api.weather.gov/points/{0}";
        private const int MAX_ATTEMPTS = 2;

        public NWSWeatherProvider() : base()
        {
            LocationProvider = new HERE.HERELocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.NWS;
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
        public override Task<Weather> GetWeather(string location_query, string country_code)
        {
            return Task.Run(async () =>
            {
                Weather weather = null;
                WeatherException wEx = null;

                try
                {
                    Uri pointsURL = new Uri(string.Format(POINTS_QUERY_URL, location_query));

                    using (var pointsRequest = new HttpRequestMessage(HttpMethod.Get, pointsURL))
                    {
                        pointsRequest.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                        // Get response
                        var webClient = SimpleLibrary.WebClient;
                        using (var cts = new CancellationTokenSource((int)(Settings.READ_TIMEOUT * 1.5f)))
                        using (var pointsResponse = await webClient.SendRequestAsync(pointsRequest).AsTask(cts.Token))
                        {
                            // Check for errors
                            CheckForErrors(pointsResponse.StatusCode);

                            Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await pointsResponse.Content.ReadAsInputStreamAsync());

                            // Load point json data
                            PointsRootobject pointsRootobject = JSONParser.Deserializer<PointsRootobject>(stream);

                            string forecastURL = pointsRootobject.forecast;
                            string forecastHourlyUrl = pointsRootobject.forecastHourly;
                            string observationStationsUrl = pointsRootobject.observationStations;

                            ForecastRootobject forecastRootobject = await GetForecastResponse(forecastURL);
                            ForecastRootobject hourlyForecastRootobject = await GetForecastResponse(forecastHourlyUrl);
                            ObservationsStationsRootobject stationsRootobject = await GetObservationStationsResponse(observationStationsUrl);

                            string stationUrl = stationsRootobject.observationStations[0];
                            ObservationsCurrentRootobject obsCurrentRootObject = await GetObservationCurrentResponse(stationUrl);

                            weather = new Weather(pointsRootobject, forecastRootobject, hourlyForecastRootobject, obsCurrentRootObject);
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

                    Logger.WriteLine(LoggerLevel.Error, ex, "NWSWeatherProvider: error getting weather data");
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
        private Task<ForecastRootobject> GetForecastResponse(string url)
        {
            return Task.Run(async () =>
            {
                ForecastRootobject root = null;

                try
                {
                    Uri weatherURL = new Uri(url + "?units=us");

                    for (int i = 0; i < MAX_ATTEMPTS; i++)
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                        {
                            request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                            try
                            {
                                // Get response
                                var webClient = SimpleLibrary.WebClient;
                                using (var cts = new CancellationTokenSource((int)(Settings.READ_TIMEOUT * 1.5f)))
                                using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                                {
                                    if (response.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        // Check for errors
                                        CheckForErrors(response.StatusCode);
                                        response.EnsureSuccessStatusCode();

                                        Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                                        // Load point json data
                                        root = JSONParser.Deserializer<ForecastRootobject>(stream);
                                    }
                                }
                            }
                            catch { }
                        }

                        if (i < MAX_ATTEMPTS - 1 && root == null)
                        {
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (WeatherException)
                {
                    // Allow continuing w/o the data
                    root = null;
                }
                catch (Exception ex)
                {
                    root = null;
                    throw ex;
                }

                return root;
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<ObservationsStationsRootobject> GetObservationStationsResponse(string url)
        {
            return Task.Run(async () =>
            {
                ObservationsStationsRootobject root = null;

                try
                {
                    Uri weatherURL = new Uri(url);

                    for (int i = 0; i < MAX_ATTEMPTS; i++)
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                        {
                            request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                            try
                            {
                                // Get response
                                var webClient = SimpleLibrary.WebClient;
                                using (var cts = new CancellationTokenSource((int)(Settings.READ_TIMEOUT * 1.5f)))
                                using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                                {
                                    if (response.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        // Check for errors
                                        CheckForErrors(response.StatusCode);
                                        response.EnsureSuccessStatusCode();

                                        Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                                        // Load point json data
                                        root = JSONParser.Deserializer<ObservationsStationsRootobject>(stream);
                                    }
                                }
                            }
                            catch { }
                        }

                        if (i < MAX_ATTEMPTS - 1 && root == null)
                        {
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    root = null;
                    throw ex;
                }

                return root;
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public Task<ObservationsCurrentRootobject> GetObservationCurrentResponse(string url)
        {
            return Task.Run(async () =>
            {
                ObservationsCurrentRootobject root = null;

                try
                {
                    Uri weatherURL = new Uri(url + "/observations/latest?require_qc=true");

                    for (int i = 0; i < MAX_ATTEMPTS; i++)
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Get, weatherURL))
                        {
                            request.Headers.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));

                            try
                            {
                                // Get response
                                var webClient = SimpleLibrary.WebClient;
                                using (var cts = new CancellationTokenSource((int)(Settings.READ_TIMEOUT * 1.5f)))
                                using (var response = await webClient.SendRequestAsync(request).AsTask(cts.Token))
                                {
                                    if (response.StatusCode == HttpStatusCode.BadRequest)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        // Check for errors
                                        CheckForErrors(response.StatusCode);
                                        response.EnsureSuccessStatusCode();

                                        Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                                        // Load point json data
                                        root = JSONParser.Deserializer<ObservationsCurrentRootobject>(stream);
                                    }
                                }
                            }
                            catch { }
                        }

                        if (i < MAX_ATTEMPTS - 1 && root == null)
                        {
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    root = null;
                    throw ex;
                }

                return root;
            });
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        private void CheckForErrors(HttpStatusCode responseCode)
        {
            switch (responseCode)
            {
                case HttpStatusCode.Ok:
                    break;
                // 400 (OK since this isn't a valid request)
                case HttpStatusCode.BadRequest:
                default:
                    throw new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
                // 404 (Not found - Invalid query)
                case HttpStatusCode.NotFound:
                    throw new WeatherException(WeatherUtils.ErrorStatus.QueryNotFound);
            }
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(LocationData location)
        {
            var weather = await base.GetWeather(location);

            var offset = location.tz_offset;
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(location.tz_offset);

            // NWS does not provide astrodata; calculate this ourselves (using their calculator)
            var solCalcData = await new SolCalcAstroProvider().GetAstronomyData(location, weather.condition.observation_time);
            weather.astronomy = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            weather.astronomy.sunrise = solCalcData.sunrise;
            weather.astronomy.sunset = solCalcData.sunset;

            return weather;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", weather.location.latitude, weather.location.longitude);
            return str;
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            var str = string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", location.latitude, location.longitude);
            return str;
        }

        public override string GetWeatherIcon(string icon)
        {
            // Example: https://api.weather.gov/icons/land/day/tsra_hi,20?size=medium
            return GetWeatherIcon(icon.Contains("/night/"), icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            // Example: https://api.weather.gov/icons/land/day/tsra_hi,20?size=medium
            string WeatherIcon = string.Empty;

            if (icon.Contains("fog"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_FOG;
                else
                    WeatherIcon = WeatherIcons.DAY_FOG;
            }
            else if (icon.Contains("blizzard"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW_WIND;
                else
                    WeatherIcon = WeatherIcons.DAY_SNOW_WIND;
            }
            else if (icon.Contains("cold"))
            {
                WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
            }
            else if (icon.Contains("hot"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_HOT;
            }
            else if (icon.Contains("haze"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_FOG;
                else
                    WeatherIcon = WeatherIcons.DAY_HAZE;
            }
            else if (icon.Contains("smoke"))
            {
                WeatherIcon = WeatherIcons.SMOKE;
            }
            else if (icon.Contains("dust"))
            {
                WeatherIcon = WeatherIcons.DUST;
            }
            else if (icon.Contains("tropical_storm") || icon.Contains("tsra") || icon.Contains("hurricane"))
            {
                WeatherIcon = WeatherIcons.HURRICANE;
            }
            else if (icon.Contains("tornado"))
            {
                WeatherIcon = WeatherIcons.TORNADO;
            }
            else if (icon.Contains("rain_showers"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SHOWERS;
                else
                    WeatherIcon = WeatherIcons.DAY_SHOWERS;
            }
            else if (icon.Contains("fzra") || icon.Contains("rain_sleet") || icon.Contains("rain_snow"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN_MIX;
                else
                    WeatherIcon = WeatherIcons.DAY_RAIN_MIX;
            }
            else if (icon.Contains("sleet"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SLEET;
                else
                    WeatherIcon = WeatherIcons.DAY_SLEET;
            }
            else if (icon.Contains("rain"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_RAIN;
                else
                    WeatherIcon = WeatherIcons.DAY_RAIN;
            }
            else if (icon.Contains("snow"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_SNOW;
                else
                    WeatherIcon = WeatherIcons.DAY_SNOW;
            }
            else if (icon.Contains("wind_bkn") || icon.Contains("wind_ovc") || icon.Contains("wind_sct"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY_WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY_WINDY;
            }
            else if (icon.Contains("wind"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.WINDY;
                else
                    WeatherIcon = WeatherIcons.DAY_WINDY;
            }
            else if (icon.Contains("ovc") || icon.Contains("sct") || icon.Contains("few"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY_OVERCAST;
            }
            else if (icon.Contains("bkn"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_CLOUDY;
                else
                    WeatherIcon = WeatherIcons.DAY_CLOUDY;
            }
            else
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_CLEAR;
                else
                    WeatherIcon = WeatherIcons.DAY_SUNNY;
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
            if (icon == null)
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_notavailable");

            if (icon.Contains("fog"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_fog");
            }
            else if (icon.Contains("blizzard"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_blizzard");
            }
            else if (icon.Contains("cold"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cold");
            }
            else if (icon.Contains("hot"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hot");
            }
            else if (icon.Contains("haze"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_haze");
            }
            else if (icon.Contains("smoke"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_smoky");
            }
            else if (icon.Contains("dust"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_dust");
            }
            else if (icon.Contains("tropical_storm") || icon.Contains("tsra"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tropicalstorm");
            }
            else if (icon.Contains("hurricane"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_hurricane");
            }
            else if (icon.Contains("tornado"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_tornado");
            }
            else if (icon.Contains("rain_showers"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainshowers");
            }
            else if (icon.Contains("fzra"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_freezingrain");
            }
            else if (icon.Contains("rain_sleet"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandsleet");
            }
            else if (icon.Contains("rain_snow"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rainandsnow");
            }
            else if (icon.Contains("sleet"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sleet");
            }
            else if (icon.Contains("rain"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_rain");
            }
            else if (icon.Contains("snow"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_snow");
            }
            else if (icon.Contains("wind_bkn") || icon.Contains("wind_ovc") || icon.Contains("wind_sct") || icon.Contains("wind"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_windy");
            }
            else if (icon.Contains("ovc"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_overcast");
            }
            else if (icon.Contains("sct") || icon.Contains("few"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_partlycloudy");
            }
            else if (icon.Contains("bkn"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_cloudy");
            }
            else if (icon.Contains("day"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_sunny");
            }
            else if (icon.Contains("night"))
            {
                return SimpleLibrary.ResLoader.GetString("/WeatherConditions/weather_clearsky");
            }
            else
            {
                return base.GetWeatherCondition(icon);
            }
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            // The following cases can be present at any time of day
            if (WeatherIcons.SNOWFLAKE_COLD.Equals(weather.condition.icon))
            {
                if (!isNight)
                {
                    // Fallback to sunset/rise time just in case
                    var sunrise = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    var sunset = NodaTime.LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);

                    var tz = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                    if (tz == null)
                        tz = NodaTime.DateTimeZone.ForOffset(NodaTime.Offset.FromTimeSpan(weather.location.tz_offset));

                    var now = NodaTime.SystemClock.Instance.GetCurrentInstant()
                                .InZone(tz).TimeOfDay;

                    // Determine whether its night using sunset/rise times
                    if (now < sunrise || now > sunset)
                        isNight = true;
                }
            }

            return isNight;
        }
    }
}