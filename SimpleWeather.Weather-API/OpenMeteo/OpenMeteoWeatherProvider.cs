using NodaTime;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Resources.Strings;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.Weather_API.NWS;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.Utils;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WAPI = SimpleWeather.WeatherData.WeatherAPI;

namespace SimpleWeather.Weather_API.OpenMeteo
{
    public partial class OpenMeteoWeatherProvider : WeatherProviderImpl, IPollenProvider
    {
        private const string NON_COMMERCIAL_FORECAST_URL = "https://api.open-meteo.com/v1/forecast";
        private const string COMMERCIAL_FORECAST_URL = "https://customer-api.open-meteo.com/v1/forecast";

        private const string NON_COMMERCIAL_AQ_URL = "https://air-quality-api.open-meteo.com/v1/air-quality";
        private const string COMMERCIAL_AQ_URL = "https://customer-air-quality-api.open-meteo.com/v1/air-quality";
        private const string KEYCHECK_QUERY_URL = $"{COMMERCIAL_FORECAST_URL}?apikey={{0}}";

        public OpenMeteoWeatherProvider() : base()
        {
            LocationProvider = this.RunCatching(() =>
            {
                return WeatherModule.Instance.LocationProviderFactory.GetLocationProvider(
                    RemoteConfigService.GetLocationProvider(WeatherAPI));
            }).GetOrElse<IWeatherLocationProvider, IWeatherLocationProvider>((t) =>
            {
                return new WeatherApi.WeatherApiLocationProvider();
            });
        }

        public override string WeatherAPI => WAPI.OpenMeteo;
        public override bool SupportsWeatherLocale => false;
        public override bool KeyRequired => false;
        public override AuthType AuthType => AuthType.Internal;

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<bool> IsKeyValid(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);

            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                this.CheckRateLimit();

                Uri queryURL = new Uri(String.Format(KEYCHECK_QUERY_URL, key));

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                using (var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT))
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

            if (wEx != null && wEx.ErrorStatus != WeatherUtils.ErrorStatus.InvalidAPIKey)
            {
                throw wEx;
            }

            return isValid;
        }

        public override string GetAPIKey()
        {
            return APIKeys.GetOpenMeteoKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        protected override async Task<Weather> GetWeatherData(SimpleWeather.LocationData.LocationData location)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = LocaleUtils.GetLocale();
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);
            var query = await UpdateLocationQuery(location);

            var key = GetProviderKey();

            try
            {
                this.CheckRateLimit();

                if ((KeyRequired || AuthType == AuthType.Internal) && String.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                ForecastResponse root = null;
                AQIResponse aqiRoot = null;

                var forecastRequestUri = COMMERCIAL_FORECAST_URL.ToUriBuilderEx()
                    .AppendQueryParameter("latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("daily",
                        "weather_code,temperature_2m_max,temperature_2m_min,uv_index_max,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,wind_direction_10m_dominant,cloud_cover_mean,apparent_temperature_mean,dew_point_2m_mean,pressure_msl_mean,relative_humidity_2m_mean,visibility_mean")
                    .AppendQueryParameter("hourly",
                        "temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,weather_code,pressure_msl,cloud_cover,visibility,wind_speed_10m,wind_gusts_10m,wind_direction_10m,rain,snowfall,uv_index,is_day")
                    .AppendQueryParameter("current",
                        "temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,weather_code,pressure_msl,cloud_cover,visibility,wind_speed_10m,wind_gusts_10m,wind_direction_10m,rain,snowfall,uv_index,is_day")
                    .AppendQueryParameter("minutely_15", "rain,snowfall")
                    .AppendQueryParameter("models", "best_match")
                    .AppendQueryParameter("timeformat", "unixtime")
                    .AppendQueryParameter("timezone", "auto")
                    .Apply(b =>
                    {
                        if ((KeyRequired || AuthType == AuthType.Internal))
                        {
                            b.AppendQueryParameter("apikey", key);
                        }
                    })
                    .BuildUri();

                var aqiRequestUri = COMMERCIAL_AQ_URL.ToUriBuilderEx()
                    .AppendQueryParameter("latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("current",
                        "us_aqi,ragweed_pollen,grass_pollen,birch_pollen,alder_pollen,pm10,pm2_5,carbon_monoxide,nitrogen_dioxide,sulphur_dioxide,ozone")
                    .AppendQueryParameter("timeformat", "unixtime")
                    .AppendQueryParameter("timezone", "auto")
                    .Apply(b =>
                    {
                        if ((KeyRequired || AuthType == AuthType.Internal))
                        {
                            b.AppendQueryParameter("apikey", key);
                        }
                    })
                    .BuildUri();

                using (var forecastRequest = new HttpRequestMessage(HttpMethod.Get, forecastRequestUri))
                {
                    forecastRequest.CacheRequestIfNeeded((KeyRequired || AuthType == AuthType.Internal), TimeSpan.FromMinutes(30));
                    forecastRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(forecastRequest, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    root = await JSONParser.DeserializerAsync<ForecastResponse>(stream) ?? throw new ArgumentNullException("root");
                }

                using (var aqiRequest = new HttpRequestMessage(HttpMethod.Get, aqiRequestUri))
                {
                    aqiRequest.CacheRequestIfNeeded((KeyRequired || AuthType == AuthType.Internal), TimeSpan.FromMinutes(20));
                    aqiRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(aqiRequest, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    aqiRoot = await JSONParser.DeserializerAsync<AQIResponse>(stream) ?? throw new ArgumentNullException("aqiRoot");
                }

                weather = this.CreateWeatherData(root, aqiRoot);
            }
            catch (Exception ex)
            {
                weather = null;

                if (ex is HttpRequestException || ex is WebException || ex is SocketException || ex is IOException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
                {
                    wEx = ex as WeatherException;
                }
                else
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather, ex);
                }

                Logger.WriteLine(LoggerLevel.Error, ex, "OpenMeteoWeatherProvider: error getting weather data");
            }

            if (weather == null || !weather.IsValid())
            {
                wEx = new WeatherException(WeatherUtils.ErrorStatus.NoWeather);
            }
            else if (weather != null)
            {
                if (SupportsWeatherLocale)
                    weather.locale = locale;

                weather.query = query;
            }

            if (wEx != null)
                throw wEx;

            return weather;
        }

        public async Task<Pollen> GetPollenData(SimpleWeather.LocationData.LocationData location)
        {
            Pollen pollenData = null;

            var key = GetProviderKey();

            try
            {
                this.CheckRateLimit();

                if ((KeyRequired || AuthType == AuthType.Internal) && string.IsNullOrWhiteSpace(key))
                {
                    throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
                }

                AQIResponse root;

                var requestUri = COMMERCIAL_AQ_URL.ToUriBuilderEx()
                    .AppendQueryParameter("latitude", location.latitude.ToString("0.####"))
                    .AppendQueryParameter("longitude", location.longitude.ToString("0.####"))
                    .AppendQueryParameter("current",
                        "ragweed_pollen,grass_pollen,birch_pollen,alder_pollen")
                    .AppendQueryParameter("timeformat", "unixtime")
                    .AppendQueryParameter("timezone", "auto")
                    .Apply(b =>
                    {
                        if ((KeyRequired || AuthType == AuthType.Internal))
                        {
                            b.AppendQueryParameter("apikey", key);
                        }
                    })
                    .BuildUri();

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.CacheRequestIfNeeded((KeyRequired || AuthType == AuthType.Internal), TimeSpan.FromMinutes(20));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(SettingsManager.READ_TIMEOUT);
                using var response = await webClient.SendAsync(request, cts.Token);

                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();

                // Load weather
                root = await JSONParser.DeserializerAsync<AQIResponse>(stream);

                root?.current?.Let((current) =>
                {
                    var treePollenMeasure = current.birch_pollen.HasValue && current.alder_pollen.HasValue ?
                        MathF.Max(current.birch_pollen.Value, current.alder_pollen.Value) : (current.birch_pollen ?? current.alder_pollen);
                    var grassPollenMeasure = current.grass_pollen;
                    var ragweedPollenMeasure = current.ragweed_pollen;

                    pollenData = new Pollen()
                    {
                        treePollenCount = treePollenMeasure switch
                        {
                            null => Pollen.PollenCount.Unknown,
                            < 15 => Pollen.PollenCount.Low,
                            < 90 => Pollen.PollenCount.Moderate,
                            < 1500 => Pollen.PollenCount.High,
                            >= 1500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        grassPollenCount = grassPollenMeasure switch
                        {
                            null => Pollen.PollenCount.Unknown,
                            < 5 => Pollen.PollenCount.Low,
                            < 20 => Pollen.PollenCount.Moderate,
                            < 200 => Pollen.PollenCount.High,
                            >= 200 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        ragweedPollenCount = ragweedPollenMeasure switch
                        {
                            null => Pollen.PollenCount.Unknown,
                            < 10 => Pollen.PollenCount.Low,
                            < 50 => Pollen.PollenCount.Moderate,
                            < 500 => Pollen.PollenCount.High,
                            >= 500 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        }
                    };
                });
            }
            catch (Exception ex)
            {
                pollenData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "OpenMeteoWeatherProvider: error getting pollen data");
            }

            return pollenData;
        }

        protected override async Task UpdateWeatherData(SimpleWeather.LocationData.LocationData location,
            Weather weather)
        {
            await base.UpdateWeatherData(location, weather);

            var offset = location.tz_offset;

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            // Calculate astronomy
            try
            {
                weather.astronomy =
                    await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            }
            catch (WeatherException)
            {
                weather.astronomy =
                    await new SolCalcAstroProvider().GetAstronomyData(location, weather.condition.observation_time);
            }

            weather.condition.icon.Let(it =>
            {
                weather.condition.icon = GetWeatherIcon(it);
                weather.condition.weather = GetWeatherCondition(it);
            });

            weather.forecast?.ForEach(forecast =>
            {
                forecast.date = forecast.date.AddSeconds(offset.TotalSeconds);

                forecast.icon.Let(it =>
                {
                    forecast.icon = GetWeatherIcon(it);
                    forecast.condition = GetWeatherCondition(it);
                });
            });

            weather.hr_forecast?.ForEach(hr_forecast =>
            {
                hr_forecast.date = hr_forecast.date.ToOffset(offset);

                hr_forecast.icon.Let(it =>
                {
                    hr_forecast.icon = GetWeatherIcon(it);
                    hr_forecast.condition = GetWeatherCondition(it);
                });
            });

            weather.min_forecast?.ForEach(minutelyForecast =>
            {
                minutelyForecast.date = minutelyForecast.date.ToOffset(offset);
            });
        }

        public override Task<string> UpdateLocationQuery(Weather weather)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}",
                weather.location.latitude, weather.location.longitude));
        }

        public override Task<string> UpdateLocationQuery(SimpleWeather.LocationData.LocationData location)
        {
            return Task.FromResult(string.Format(CultureInfo.InvariantCulture, "latitude={0:0.####}&longitude={1:0.####}",
                location.latitude, location.longitude));
        }

        public override string GetWeatherIcon(string icon)
        {
            var isNight = false;

            if (icon == null) return WeatherIcons.NA;

            if (icon.EndsWith("_0")) isNight = true;

            return GetWeatherIcon(isNight, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            var conditionCode = NumberUtils.TryParseInt(icon.Split('_').FirstOrDefault());
            if (conditionCode == null)
                return WeatherIcons.NA;

            string weatherIcon = conditionCode switch
            {
                /*
                 * 0: Clear sky
                 * 1: Mainly clear
                 */
                0 or 1 => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,

                /*
                 * 2: Partly cloudy
                 */
                2 => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,

                /*
                 * 3: overcast
                 */
                3 => WeatherIcons.OVERCAST,

                /*
                 * 45: Fog
                 * 48: depositing rime fog
                 */
                45 or 48 => WeatherIcons.FOG,

                /*
                 * 51, 53, 55: Drizzle: Light, moderate, and dense intensity
                 */
                51 or 53 or 55 => WeatherIcons.SPRINKLE,

                /*
                 * 56, 57: Freezing Drizzle: Light and dense intensity
                 */
                56 or 57 => WeatherIcons.RAIN_MIX,

                /*
                 * 61, 63: Rain: Slight and moderate intensity
                 */
                61 or 63 => WeatherIcons.RAIN,

                /*
                 * 65: Rain: Heavy intensity
                 */
                65 => WeatherIcons.RAIN_WIND,

                /*
                 * 66, 67: Freezing Rain: Light and heavy intensity
                 */
                66 or 67 => WeatherIcons.RAIN_MIX,

                /*
                 * 71, 73, 77: Snow fall: Slight, moderate, and heavy intensity; Snow grains
                 */
                71 or 73 or 77 => WeatherIcons.SNOW,

                /*
                 * 75: Snow fall: Heavy intensity
                 */
                75 => WeatherIcons.SNOW_WIND,

                /*
                 * 80, 81, 82: Rain showers: Slight, moderate, and violent
                 */
                80 or 81 => WeatherIcons.SHOWERS,
                82 => WeatherIcons.STORM_SHOWERS,

                /*
                 * 85, 86: Snow showers slight and heavy
                 */
                85 => WeatherIcons.SNOW,
                86 => WeatherIcons.SNOW_WIND,

                /*
                 * 95: Thunderstorm: Slight or moderate
                 * 96, 99: Thunderstorm with slight and heavy hail
                 */
                95 => WeatherIcons.THUNDERSTORM,
                96 or 99 => WeatherIcons.SNOW_THUNDERSTORM,

                _ => string.Empty,
            };

            if (string.IsNullOrWhiteSpace(weatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                weatherIcon = WeatherIcons.NA;
            }

            return weatherIcon;
        }

        public override string GetWeatherCondition(string icon)
        {
            var conditionCode = NumberUtils.TryParseInt(icon?.Split('_')?.FirstOrDefault());
            if (conditionCode == null)
                return base.GetWeatherCondition(icon);

            return conditionCode switch
            {
                /* 0: Clear sky */
                0 => WeatherConditions.weather_clearsky,
                /* 1: Mainly clear */
                1 => WeatherConditions.weather_mostlyclear,
                /* 2: partly cloudy */
                2 => WeatherConditions.weather_partlycloudy,
                /* 3: overcast */
                3 => WeatherConditions.weather_overcast,
                /* 45, 48: Fog and depositing rime fog */
                45 or 48 => WeatherConditions.weather_fog,
                /* 51, 53, 55: Drizzle: Light, moderate, and dense intensity */
                51 or 53 or 55 => WeatherConditions.weather_drizzle,
                /*
                 * 56, 57: Freezing Drizzle: Light and dense intensity
                 * 66, 67: Freezing Rain: Light and heavy intensity
                 */
                56 or 57 or 66 or 67 => WeatherConditions.weather_freezingrain,
                /* 61, 63, 65: Rain: Slight, moderate and heavy intensity */
                61 => WeatherConditions.weather_lightrain,
                63 => WeatherConditions.weather_rain,
                65 => WeatherConditions.weather_heavyrain,
                /*
                 * 71, 73, 75: Snow fall: Slight, moderate, and heavy intensity
                 * 77: Snow grains
                 */
                71 => WeatherConditions.weather_lightsnowshowers,
                73 or 77 => WeatherConditions.weather_snow,
                75 => WeatherConditions.weather_heavysnow,
                /* 80, 81, 82: Rain showers: Slight, moderate, and violent */
                80 or 81 or 82 => WeatherConditions.weather_rainshowers,
                /* 85, 86: Snow showers slight and heavy */
                85 => WeatherConditions.weather_lightsnowshowers,
                86 => WeatherConditions.weather_heavysnow,
                /*
                 * 95, 96, 99: Thunderstorm: Slight/moderate; with slight/heavy hail
                 */
                95 or 96 or 99 => WeatherConditions.weather_tstorms,
                _ => this.Let(_ =>
                {
                    this.LogMissingIcon(icon);
                    return WeatherConditions.weather_notavailable;
                }),
            };
        }

        // Some conditions can be for any time of day
        // So use sunrise/set data as fallback
        public override bool IsNight(Weather weather)
        {
            bool isNight = base.IsNight(weather);

            if (!isNight)
            {
                // Fallback to sunset/rise time just in case
                LocalTime sunrise;
                LocalTime sunset;
                if (weather.astronomy != null)
                {
                    sunrise = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunrise.TimeOfDay.Ticks);
                    sunset = LocalTime.FromTicksSinceMidnight(weather.astronomy.sunset.TimeOfDay.Ticks);
                }
                else
                {
                    sunrise = LocalTime.FromHourMinuteSecondTick(6, 0, 0, 0);
                    sunset = LocalTime.FromHourMinuteSecondTick(18, 0, 0, 0);
                }

                DateTimeZone tz = null;

                if (weather.location.tz_long != null)
                {
                    tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(weather.location.tz_long);
                }

                if (tz == null)
                    tz = DateTimeZone.ForOffset(Offset.FromTimeSpan(weather.location.tz_offset));

                var now = SystemClock.Instance.GetCurrentInstant()
                    .InZone(tz).TimeOfDay;

                // Determine whether its night using sunset/rise times
                if (now < sunrise || now > sunset)
                    isNight = true;
            }

            return isNight;
        }
    }
}