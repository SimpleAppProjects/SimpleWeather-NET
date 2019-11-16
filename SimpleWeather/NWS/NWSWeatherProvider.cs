using SimpleWeather.Location;
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

            Uri weatherURL = null;

            var handler = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = true
            };

            HttpClient webClient = new HttpClient(handler);

            WeatherException wEx = null;

            try
            {
                string queryAPI = "https://api.weather.gov/points/{0}";
                weatherURL = new Uri(string.Format(queryAPI, location_query));

                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL).AsTask(cts.Token);
                // Check for errors
                CheckForErrors(response.StatusCode);

                Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                // Reset exception
                wEx = null;

                // Load point json data
                PointsRootobject pointsRootobject = null;
                await Task.Run(() =>
                {
                    pointsRootobject = JSONParser.Deserializer<PointsRootobject>(stream);
                });

                // End Stream
                if (stream != null)
                    stream.Dispose();
                cts.Dispose();

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
            catch (Exception ex)
            {
                weather = null;

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "NWSWeatherProvider: error getting weather data");
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

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public async Task<ForecastRootobject> GetForecastResponse(string url)
        {
            ForecastRootobject root = null;

            Uri weatherURL = null;

            var handler = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = true
            };

            HttpClient webClient = new HttpClient(handler);

            try
            {
                weatherURL = new Uri(url + "?units=us");

                var version = string.Format("v{0}.{1}.{2}",
                    Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                CancellationTokenSource cts = new CancellationTokenSource(Settings.READ_TIMEOUT);

                // Get response
                HttpResponseMessage response = await webClient.GetAsync(weatherURL).AsTask(cts.Token);
                // Check for errors
                CheckForErrors(response.StatusCode);
                response.EnsureSuccessStatusCode();

                Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                // Load point json data
                await Task.Run(() =>
                {
                    root = JSONParser.Deserializer<ForecastRootobject>(stream);
                });

                // End Stream
                if (stream != null)
                    stream.Dispose();
                cts.Dispose();
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
            finally
            {
                // End Stream
                webClient.Dispose();
                handler.Dispose();
            }

            return root;
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public async Task<ObservationsStationsRootobject> GetObservationStationsResponse(string url)
        {
            ObservationsStationsRootobject root = null;

            Uri weatherURL = null;

            using (var handler = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = true
            })
            using (HttpClient webClient = new HttpClient(handler))
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    weatherURL = new Uri(url);

                    var version = string.Format("v{0}.{1}.{2}",
                        Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                    webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                    webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL).AsTask(cts.Token);
                    // Check for errors
                    CheckForErrors(response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // Load point json data
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<ObservationsStationsRootobject>(stream);
                    });

                    // End Stream
                    if (stream != null)
                        stream.Dispose();
                }
                catch (Exception ex)
                {
                    root = null;
                    throw ex;
                }
                finally
                {
                    // End Stream
                    webClient.Dispose();
                }

                return root;
            }
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public async Task<ObservationsCurrentRootobject> GetObservationCurrentResponse(string url)
        {
            ObservationsCurrentRootobject root = null;

            Uri weatherURL = null;

            using (var handler = new HttpBaseProtocolFilter()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = true
            })
            using (HttpClient webClient = new HttpClient(handler))
            using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
            {
                try
                {
                    weatherURL = new Uri(url + "/observations/latest");

                    var version = string.Format("v{0}.{1}.{2}",
                        Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);

                    webClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/ld+json"));
                    webClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("SimpleWeather (thewizrd.dev@gmail.com)", version));

                    // Get response
                    HttpResponseMessage response = await webClient.GetAsync(weatherURL).AsTask(cts.Token);
                    // Check for errors
                    CheckForErrors(response.StatusCode);
                    response.EnsureSuccessStatusCode();

                    Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());

                    // Load point json data
                    await Task.Run(() =>
                    {
                        root = JSONParser.Deserializer<ObservationsCurrentRootobject>(stream);
                    });

                    // End Stream
                    stream?.Dispose();
                }
                catch (Exception ex)
                {
                    root = null;
                    throw ex;
                }
                finally
                {
                    // End Stream
                    webClient.Dispose();
                }

                return root;
            }
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

            return weather;
        }

        public override string UpdateLocationQuery(Weather weather)
        {
            return string.Format("{0},{1}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format("{0},{1}", location.latitude.ToString(CultureInfo.InvariantCulture), location.longitude.ToString(CultureInfo.InvariantCulture));
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
            else if (icon.Contains("tropical_storm") || icon.Contains("tsra"))
            {
                if (isNight)
                    WeatherIcon = WeatherIcons.NIGHT_ALT_THUNDERSTORM;
                else
                    WeatherIcon = WeatherIcons.DAY_THUNDERSTORM;
            }
            else if (icon.Contains("hurricane"))
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