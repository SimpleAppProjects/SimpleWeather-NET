using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;

namespace SimpleWeather.TomorrowIO
{
    public partial class TomorrowIOWeatherProvider : WeatherProviderImpl, IPollenProvider
    {
        private const String BASE_URL = "https://api.tomorrow.io/v4/timelines";
        private const String EVENTS_BASE_URL = "https://api.tomorrow.io/v4/events";
        private const String KEYCHECK_QUERY_URL = BASE_URL + "?apikey={0}";

        public TomorrowIOWeatherProvider() : base()
        {
            LocationProvider = RemoteConfig.RemoteConfig.GetLocationProvider(WeatherAPI);
            if (LocationProvider == null)
            {
                LocationProvider = new Bing.BingMapsLocationProvider();
            }
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.TomorrowIo;
        public override bool SupportsWeatherLocale => true;
        public override bool SupportsAlerts => true;
        public override bool NeedsExternalAlertData => false;
        public override bool KeyRequired => true;
        public override int HourlyForecastInterval => 1;

        public override long GetRetryTime()
        {
            return 5000;
        }

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
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
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
            return APIKeys.GetTomorrowIOKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = Settings.UsePersonalKey ? Settings.APIKeys[WeatherData.WeatherAPI.TomorrowIo] : GetAPIKey();

            try
            {
                this.CheckRateLimit();

                Rootobject root;
                Rootobject minutelyRoot = null;
                AlertRootobject alertsRoot = null;

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("fields", "temperature,temperatureApparent,temperatureMin,temperatureMax,dewPoint,humidity,windSpeed,windDirection,windGust,pressureSeaLevel,precipitationIntensity,precipitationProbability,snowAccumulation,sunriseTime,sunsetTime,visibility,cloudCover,moonPhase,weatherCode,weatherCodeFullDay,weatherCodeDay,weatherCodeNight,treeIndex,grassIndex,weedIndex,epaIndex")
                    .AppendQueryParameter("timesteps", "current,1h,1d")
                    .AppendQueryParameter("units", "metric")
                    .AppendQueryParameter("timezone", "UTC")
                    .BuildUri();

                var minutelyRequestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("fields", "precipitationIntensity,precipitationProbability")
                    .AppendQueryParameter("timesteps", "1m")
                    .AppendQueryParameter("units", "metric")
                    .AppendQueryParameter("timezone", "UTC")
                    .BuildUri();

                var alertsRequestUri = EVENTS_BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", location_query)
                    .AppendQueryParameter("insights", "air")
                    .AppendQueryParameter("insights", "fires")
                    .AppendQueryParameter("insights", "wind")
                    .AppendQueryParameter("insights", "winter")
                    .AppendQueryParameter("insights", "thunderstorms")
                    .AppendQueryParameter("insights", "floods")
                    .AppendQueryParameter("insights", "temperature")
                    .AppendQueryParameter("insights", "tropical")
                    .AppendQueryParameter("insights", "marine")
                    .AppendQueryParameter("insights", "fog")
                    .AppendQueryParameter("insights", "tornado")
                    .AppendQueryParameter("buffer", "20")
                    .BuildUri();

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(20));
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(request, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    root = await JSONParser.DeserializerAsync<Rootobject>(stream);
                }

                try
                {
                    using (var minutelyRequest = new HttpRequestMessage(HttpMethod.Get, minutelyRequestUri))
                    {
                        minutelyRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));
                        minutelyRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Get response
                        var webClient = SharedModule.Instance.WebClient;
                        using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                        using var response = await webClient.SendAsync(minutelyRequest, cts.Token);

                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        using var stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        minutelyRoot = await JSONParser.DeserializerAsync<Rootobject>(stream);
                    }
                }
                catch { }

                try
                {
                    using (var alertsRequest = new HttpRequestMessage(HttpMethod.Get, alertsRequestUri))
                    {
                        alertsRequest.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));
                        alertsRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // Get response
                        var webClient = SharedModule.Instance.WebClient;
                        using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                        using var response = await webClient.SendAsync(alertsRequest, cts.Token);

                        await this.CheckForErrors(response);
                        response.EnsureSuccessStatusCode();

                        using var stream = await response.Content.ReadAsStreamAsync();

                        // Load weather
                        alertsRoot = await JSONParser.DeserializerAsync<AlertRootobject>(stream);
                    }
                }
                catch { }

                weather = new Weather(root, minutelyRoot, alertsRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "TomorrowIOWeatherProvider: error getting weather data");
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

        public async Task<Pollen> GetPollenData(LocationData location)
        {
            Pollen pollenData = null;

            var key = Settings.APIKeys[WeatherData.WeatherAPI.TomorrowIo] ?? GetAPIKey();
            if (String.IsNullOrWhiteSpace(key)) return null;

            try
            {
                this.CheckRateLimit();

                Rootobject root;

                var requestUri = BASE_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("location", UpdateLocationQuery(location))
                    .AppendQueryParameter("fields", "treeIndex,grassIndex,weedIndex")
                    .AppendQueryParameter("timesteps", "current")
                    .AppendQueryParameter("units", "metric")
                    .AppendQueryParameter("timezone", "UTC")
                    .BuildUri();

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Get response
                var webClient = SharedModule.Instance.WebClient;
                using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                using var response = await webClient.SendAsync(request, cts.Token);

                await this.CheckForErrors(response);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();

                // Load weather
                root = await JSONParser.DeserializerAsync<Rootobject>(stream);

                root?.data?.timelines?.FirstOrDefault()?.intervals?.FirstOrDefault()?.Let((item) =>
                {
                    pollenData = new Pollen()
                    {
                        treePollenCount = item.values.treeIndex switch
                        {
                            1 or 2 => Pollen.PollenCount.Low,
                            3 => Pollen.PollenCount.Moderate,
                            4 => Pollen.PollenCount.High,
                            5 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        grassPollenCount = item.values.grassIndex switch
                        {
                            1 or 2 => Pollen.PollenCount.Low,
                            3 => Pollen.PollenCount.Moderate,
                            4 => Pollen.PollenCount.High,
                            5 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        },
                        ragweedPollenCount = item.values.weedIndex switch
                        {
                            1 or 2 => Pollen.PollenCount.Low,
                            3 => Pollen.PollenCount.Moderate,
                            4 => Pollen.PollenCount.High,
                            5 => Pollen.PollenCount.VeryHigh,
                            _ => Pollen.PollenCount.Unknown
                        }
                    };
                });
            }
            catch (Exception ex)
            {
                pollenData = null;
                Logger.WriteLine(LoggerLevel.Error, ex, "TomorrowIOWeatherProvider: error getting pollen data");
            }

            return pollenData;
        }

        protected override async Task UpdateWeatherData(LocationData location, WeatherData.Weather weather)
        {
            var offset = location.tz_offset;

            // Update tz for weather properties
            weather.update_time = weather.update_time.ToOffset(offset);
            weather.condition.observation_time = weather.condition.observation_time.ToOffset(offset);

            var newAstro = await new SunMoonCalcProvider().GetAstronomyData(location, weather.condition.observation_time);
            if (weather.astronomy == null)
            {
                weather.astronomy = newAstro;
            }
            else
            {
                weather.astronomy.sunrise = weather.astronomy.sunrise.AddSeconds(offset.TotalSeconds);
                weather.astronomy.sunset = weather.astronomy.sunset.AddSeconds(offset.TotalSeconds);
                weather.astronomy.moonrise = newAstro.moonrise;
                weather.astronomy.moonset = newAstro.moonset;
            }

            // Update icons
            var now = DateTimeOffset.UtcNow.ToOffset(offset).TimeOfDay;
            var sunrise = weather.astronomy.sunrise.TimeOfDay;
            var sunset = weather.astronomy.sunset.TimeOfDay;

            weather.condition.icon = GetWeatherIcon(now < sunrise || now > sunset, weather.condition.icon);
            weather.condition.weather = GetWeatherCondition(weather.condition.icon);

            foreach (Forecast forecast in weather.forecast)
            {
                forecast.date = forecast.date.AddSeconds(offset.TotalSeconds);
                forecast.icon = GetWeatherIcon(forecast.icon);
                forecast.condition = GetWeatherCondition(forecast.icon);
            }

            foreach (HourlyForecast hr_forecast in weather.hr_forecast)
            {
                var hrf_date = hr_forecast.date.ToOffset(offset);
                hr_forecast.date = hrf_date;

                var hrf_localTime = hrf_date.DateTime.TimeOfDay;
                hr_forecast.icon = GetWeatherIcon(hrf_localTime < sunrise || hrf_localTime > sunset, hr_forecast.icon);
                hr_forecast.condition = GetWeatherCondition(hr_forecast.icon);
            }

            if (weather.min_forecast?.Any() == true)
            {
                foreach (var min_forecast in weather.min_forecast)
                {
                    min_forecast.date = min_forecast.date.ToOffset(offset);
                }
            }

            if (weather.weather_alerts?.Any() == true)
            {
                foreach (var alert in weather.weather_alerts)
                {
                    if (!alert.Date.Offset.Equals(offset))
                    {
                        alert.Date = new DateTimeOffset(alert.Date.DateTime, offset);
                    }

                    if (!alert.ExpiresDate.Offset.Equals(offset))
                    {
                        alert.ExpiresDate = new DateTimeOffset(alert.ExpiresDate.DateTime, offset);
                    }
                }
            }
        }

        public override string UpdateLocationQuery(WeatherData.Weather weather)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", weather.location.latitude, weather.location.longitude);
        }

        public override string UpdateLocationQuery(LocationData location)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.####},{1:0.####}", location.latitude, location.longitude);
        }

        public override string GetWeatherIcon(string icon)
        {
            return GetWeatherIcon(false, icon);
        }

        public override string GetWeatherIcon(bool isNight, string icon)
        {
            if (icon == null || !int.TryParse(icon, out int conditionCode))
                return WeatherIcons.NA;

            string WeatherIcon = conditionCode switch
            {
                /* Sunny / Clear */
                1000 => isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY,
                /*
                 * 1001: Cloudy
                 */
                1001 => WeatherIcons.CLOUDY,
                /*
                 * 1101: Partly cloudy
                 * 1100: Mostly Clear
                 * 1103
                 * Mixed conditions:
                 * Condition 1: Partly Cloudy
                 * Condition 2: Mostly Clear
                 */
                1100 or 1101 or 1103 => isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY,
                /*
                 * 1102: Mostly Cloudy
                 */
                1102 => isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY,
                /*
                 * 2000: Fog
                 * 2100: Light fog
                 */
                2000 or 2100 => WeatherIcons.FOG,
                /*
                 * 3000: Light Wind
                 */
                3000 => isNight ? WeatherIcons.NIGHT_LIGHT_WIND : WeatherIcons.DAY_LIGHT_WIND,
                /*
                 * 3001: Wind
                 */
                3001 => isNight ? WeatherIcons.NIGHT_WINDY : WeatherIcons.DAY_WINDY,
                /* Strong Wind */
                3002 => WeatherIcons.STRONG_WIND,
                /*
                 * 4000: Drizzle
                 * 4200: Light rain
                 */
                4000 or 4200 => WeatherIcons.SPRINKLE,
                /* Rain */
                4001 => WeatherIcons.RAIN,
                /* Heavy rain */
                4201 => WeatherIcons.RAIN_WIND,
                /*
                 * 5001: Flurries
                 * 5100: Light snow
                 * 5000: Snow
                 */
                5001 or 5100 or 5000 => WeatherIcons.SNOW,
                /* Heavy snow */
                5101 => WeatherIcons.SNOW_WIND,
                /*
                 * 6000: Freezing Drizzle
                 * 6200: Light Freezing Drizzle
                 * 6001: Freezing Rain
                 * 6201: Heavy Freezing Rain
                 */
                6000 or 6200 or 6001 or 6201 => WeatherIcons.RAIN_MIX,
                /*
                 * 7102: Light Ice pellets
                 * 7000: Ice pellets
                 * 7101: Heavy Ice pellets
                 */
                7102 or 7000 or 7101 => WeatherIcons.HAIL,
                /* Thunderstorm  */
                8000 => WeatherIcons.THUNDERSTORM,

                /* Mixed Condition Codes */
                /* 10000, 10001 - Clear */
                10000 => WeatherIcons.DAY_SUNNY,
                10001 => WeatherIcons.NIGHT_CLEAR,

                /*
                 * 11000, 11001 - Mostly Clear
                 * 11010, 11011 - Partly Cloudy
                 */
                11000 or 11010 => WeatherIcons.DAY_PARTLY_CLOUDY,
                11001 or 11011 => WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY,

                /* 11030: Partly Cloudy */
                11030 => WeatherIcons.DAY_PARTLY_CLOUDY,

                /* 11020, 11021 - Mostly Cloudy */
                11020 => WeatherIcons.DAY_CLOUDY,
                11021 => WeatherIcons.NIGHT_ALT_CLOUDY,

                /* 10010, 10011: Cloudy */
                10010 or 10011 => WeatherIcons.CLOUDY,

                /* 11031: Mostly Clear */
                11031 => WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY,

                /*
                 * 21000, 21001 -> Light Fog
                 * 20000, 20001 -> Fog
                 */
                21000 or 21001 or 20000 or 20001 => WeatherIcons.FOG,

                /*
                 * Full  D      Nt
                 * 2101, 21010, 21011
                 * 2102, 21020, 21021
                 * 2103, 21030, 21031
                 * 2106, 21060, 21061
                 * 2107, 21070, 21071
                 * 2108, 21080, 21081
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Light Fog / Fog
                 */
                2101 or 2102 or 2103 or 2106 or 2107 or 2108 => isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG,
                21010 or 21020 or 21030 or 21060 or 21070 or 21080 => WeatherIcons.DAY_FOG,
                21011 or 21021 or 21031 or 21061 or 21071 or 21081 => WeatherIcons.NIGHT_FOG,

                /*
                 * 40000, 40001: Drizzle
                 * 42000, 42001: Light Rain
                 */
                40000 or 40001 or 42000 or 42001 => WeatherIcons.SPRINKLE,

                /*
                 * Full  D      Nt
                 * 4203, 42030, 42031
                 * 4204, 42040, 42041
                 * 4205, 42050, 42051
                 * 4213, 42130, 42131
                 * 4214, 42140, 42141
                 * 4215, 42150, 42151
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Drizzle / Light Rain
                 */
                4203 or 4204 or 4205 or 4213 or 4214 or 4215 => isNight ? WeatherIcons.NIGHT_ALT_SPRINKLE : WeatherIcons.DAY_SPRINKLE,
                42030 or 42040 or 42050 or 42130 or 42140 or 42150 => WeatherIcons.DAY_SPRINKLE,
                42031 or 42041 or 42051 or 42131 or 42141 or 42151 => WeatherIcons.NIGHT_ALT_SPRINKLE,

                /*
                 * 40010, 40011: Rain
                 */
                40010 or 40011 => WeatherIcons.RAIN,

                /*
                 * Full  D      Nt
                 * 4209, 42090, 42091
                 * 4208, 42080, 42081
                 * 4210, 42100, 42101
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Rain
                 */
                4209 or 4208 or 4210 => isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN,
                42090 or 42080 or 42100 => WeatherIcons.DAY_RAIN,
                42091 or 42081 or 42101 => WeatherIcons.NIGHT_ALT_RAIN,

                /*
                 * 42010, 42011: Heavy Rain
                 */
                42010 or 42011 => WeatherIcons.RAIN_WIND,

                /*
                 * Full  D      Nt
                 * 4211, 42110, 42111
                 * 4202, 42020, 42021
                 * 4212, 42120, 42121
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Heavy Rain
                 */
                4211 or 4202 or 4212 => isNight ? WeatherIcons.NIGHT_ALT_RAIN_WIND : WeatherIcons.DAY_RAIN_WIND,
                42110 or 42020 or 42120 => WeatherIcons.DAY_RAIN_WIND,
                42111 or 42021 or 42121 => WeatherIcons.NIGHT_ALT_RAIN_WIND,

                /*
                 * 50010, 50011: Flurries
                 * 51000, 51001: Light snow
                 * 50000, 50001: Snow
                 */
                50010 or 50011 or
                51000 or 51001 or
                50000 or 50001 => WeatherIcons.SNOW,

                /*
                 * Full  D      Nt
                 * 5115, 51150, 51151
                 * 5116, 51160, 51161
                 * 5117, 51170, 51171
                 * 5102, 51020, 51021
                 * 5103, 51030, 51031
                 * 5104, 51040, 51041
                 * 5105, 51050, 51051
                 * 5106, 51060, 51061
                 * 5107, 51070, 51071
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Flurries / Light Snow / Snow
                 */
                5115 or 5116 or 5117 or 5102 or 5103 or 5104 or 5105 or 5106 or 5107 => isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW,
                51150 or 51160 or 51170 or 51020 or 51030 or 51040 or 51050 or 51060 or 51070 => WeatherIcons.DAY_SNOW,
                51151 or 51161 or 51171 or 51021 or 51031 or 51041 or 51051 or 51061 or 51071 => WeatherIcons.NIGHT_ALT_SNOW,

                /*
                 * 5122, 51220, 51221
                 * 5110, 51100, 51101
                 * 5108, 51080, 51081
                 * 5114, 51140, 51141
                 * 6204, 62040, 62041
                 * 6206, 62060, 62061
                 * 6212, 62120, 62121
                 * 6220, 62200, 62201
                 * 6222, 62220, 62221
                 * Mixed conditions:
                 * Condition 1: Drizzle / Rain / Light Rain
                 * Condition 2: Light Snow / Snow / Freezing Rain / Freezing Drizzle
                 *
                 * 60000, 60001: Freezing Drizzle
                 * 62000, 62001: Light Freezing Drizzle
                 * 60010, 60011: Freezing Rain
                 * 62010, 62011: Heavy Freezing Rain
                 */
                5122 or 51220 or 51221 or
                5110 or 51100 or 51101 or
                5108 or 51080 or 51081 or
                5114 or 51140 or 51141 or
                60000 or 60001 or
                62000 or 62001 or
                60010 or 60011 or
                62010 or 62011 or
                6204 or 62040 or 62041 or
                6206 or 62060 or 62061 or
                6212 or 62120 or 62121 or
                6220 or 62200 or 62201 or
                6222 or 62220 or 62221 => WeatherIcons.RAIN_MIX,

                /*
                 * Full  D      Nt
                 * 6003, 60030, 60031
                 * 6002, 60020, 60021
                 * 6004, 60040, 60041
                 * 6205, 62050, 62051
                 * 6203, 62030, 62031
                 * 6209, 62090, 62091
                 * 6213, 62130, 62131
                 * 6214, 62140, 62141
                 * 6215, 62150, 62151
                 * 6207, 62070, 62071
                 * 6202, 62020, 62021
                 * 6208, 62080, 62081
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Freezing Drizzle / Light Freezing Rain / Freezing Rain / Heavy Freezing Rain
                 */
                6003 or 6002 or 6004 or 6205 or 6203 or 6209 or 6213 or 6214 or 6215 or 6207 or 6202 or 6208 => isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX,
                60030 or 60020 or 60040 or 62050 or 62030 or 62090 or 62130 or 62140 or 62150 or 62070 or 62020 or 62080 => WeatherIcons.DAY_RAIN_MIX,
                60031 or 60021 or 60041 or 62051 or 62031 or 62091 or 62131 or 62141 or 62151 or 62071 or 62021 or 62081 => WeatherIcons.NIGHT_ALT_RAIN_MIX,

                /*
                 * 51010, 51011: Heavy snow
                 */
                51010 or 51011 => WeatherIcons.SNOW_WIND,

                /*
                 * Full  D      Nt
                 * 5119, 51190, 51191
                 * 5120, 51200, 51201
                 * 5121, 51210, 51211
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Heavy Snow
                 */
                5119 or 5120 or 5121 => isNight ? WeatherIcons.NIGHT_ALT_SNOW_WIND : WeatherIcons.DAY_SNOW_WIND,
                51190 or 51200 or 51210 => WeatherIcons.DAY_SNOW_WIND,
                51191 or 51201 or 51211 => WeatherIcons.NIGHT_ALT_SNOW_WIND,

                /*
                 * 5112, 51120, 51121
                 * Mixed conditions:
                 * Condition 1: Snow
                 * Condition 2: Ice Pellets
                 *
                 * 71020, 71021: Light Ice Pellets
                 * 70000, 70001: Ice Pellets
                 * 71010, 71011: Heavy Ice Pellets
                 */
                5112 or 51120 or 51121 or
                71020 or 71021 or
                70000 or 70001 or
                71010 or 71011 => WeatherIcons.HAIL,

                /*
                 * Full  D      Nt
                 * 7110, 71100, 71101
                 * 7111, 71110, 71111
                 * 7112, 71120, 71121
                 * 7108, 71080, 71081
                 * 7107, 71070, 71071
                 * 7109, 71090, 71091
                 * 7113, 71130, 71131
                 * 7114, 71140, 71141
                 * 7116, 71160, 71161
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Light Ice Pellets / Ice Pellets / Heavy Ice Pellets
                 */
                7110 or 7111 or 7112 or 7108 or 7107 or 7109 or 7113 or 7114 or 7116 => isNight ? WeatherIcons.NIGHT_ALT_HAIL : WeatherIcons.DAY_HAIL,
                71100 or 71110 or 71120 or 71080 or 71070 or 71090 or 71130 or 71140 or 71160 => WeatherIcons.DAY_HAIL,
                71101 or 71111 or 71121 or 71081 or 71071 or 71091 or 71131 or 71141 or 71161 => WeatherIcons.NIGHT_ALT_HAIL,

                /*
                 * 7105, 71050, 71051
                 * 7115, 71150, 71151
                 * 7117, 71170, 71171
                 * 7106, 71060, 71061
                 * 7103, 71030, 71031
                 * Mixed conditions:
                 * Condition 1: Drizzle / Rain / Light Rain / Freezing Rain
                 * Condition 2: Ice Pellets / Heavy Ice Pellets
                 */
                7105 or 71050 or 71051 or
                7115 or 71150 or 71151 or
                7117 or 71170 or 71171 or
                7106 or 71060 or 71061 or
                7103 or 71030 or 71031 => WeatherIcons.RAIN_MIX,

                /*
                 * 80000, 80001: Thunderstorm
                 */
                80000 or 80001 => WeatherIcons.THUNDERSTORM,

                /*
                 * Full  D      Nt
                 * 8001, 80010, 80011
                 * 8003, 80030, 80031
                 * 8002, 80020, 80021
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Thunderstorm
                 */
                8001 or 8003 or 8002 => isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM,
                80010 or 80030 or 80020 => WeatherIcons.DAY_THUNDERSTORM,
                80011 or 80031 or 80021 => WeatherIcons.NIGHT_ALT_THUNDERSTORM,

                _ => string.Empty,
            };

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }

        public override string GetWeatherCondition(string icon)
        {
            var conditionCode = icon.TryParseInt();

            if (!conditionCode.HasValue)
            {
                return base.GetWeatherCondition(icon);
            }

            return conditionCode.Value switch
            {
                /* Clear */
                1000 or 10000 or 10001 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_clear"),
                /* Mostly Clear */
                1100 or 11000 or 11001 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_mostlyclear"),
                /* Partly Cloudy */
                1101 or 11010 or 11011 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_partlycloudy"),
                /* Mostly Cloudy */
                1102 or 11020 or 11021 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_mostlycloudy"),
                /* Cloudy */
                1001 or 10010 or 10011 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_cloudy"),
                /* Mostly Clear */
                1103 or 11030 or 11031 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_mostlyclear"),
                /* Light Fog */
                2100 or 21000 or 21001 or
                2101 or 21010 or 21011 or
                2102 or 21020 or 21021 or
                2103 or 21030 or 21031 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_lightfog"),
                /* Fog */
                2000 or 20000 or 20001 or
                2106 or 21060 or 21061 or
                2107 or 21070 or 21071 or
                2108 or 21080 or 21081 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_fog"),
                /* Drizzle */
                4000 or 40000 or 40001 or
                4203 or 42030 or 42031 or
                4204 or 42040 or 42041 or
                4205 or 42050 or 42051 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_drizzle"),
                /* Light Rain */
                4200 or 42000 or 42001 or
                4213 or 42130 or 42131 or
                4214 or 42140 or 42141 or
                4215 or 42150 or 42151 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_lightrain"),
                /* Rain */
                4001 or 40010 or 40011 or
                4209 or 42090 or 42091 or
                4208 or 42080 or 42081 or
                4210 or 42100 or 42101 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rain"),
                /* Heavy Rain */
                4201 or 42010 or 42011 or
                4211 or 42110 or 42111 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_heavyrain"),
                /* Flurries */
                5001 or 50010 or 50011 or
                5115 or 51150 or 51151 or
                5116 or 51160 or 51161 or
                5117 or 51170 or 51171 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snowflurries"),
                /* Light Snow */
                5100 or 51000 or 51001 or
                5102 or 51020 or 51021 or
                5103 or 51030 or 51031 or
                5104 or 51040 or 51041 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_lightsnowshowers"),
                /* Snow */
                5000 or 50000 or 50001 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Snow
                 */
                5105 or 51050 or 51051 or
                5106 or 51060 or 51061 or
                5107 or 51070 or 51071 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snow"),
                /* Heavy Snow */
                5101 or 51010 or 51011 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Snow
                 */
                5119 or 51190 or 51191 or
                5120 or 51200 or 51201 or
                5121 or 51210 or 51211 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_heavysnow"),
                /*
                 * Mixed conditions:
                 * Condition 1: Drizzle / Rain
                 * Condition 2: Light Snow / Snow
                 */
                5122 or 51220 or 51221 or
                5110 or 51100 or 51101 or
                5108 or 51080 or 51081 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rainandsnow"),
                /*
                 * Mixed conditions:
                 * Condition 1: Snow
                 * Condition 2: Freezing Rain
                 */
                5114 or 51140 or 51141 or
                /* Freezing Drizzle / Light Freezing Drizzle / Freezing Rain / Heavy Freezing Rain */
                6000 or 60000 or 60001 or
                6200 or 62000 or 62001 or
                6001 or 60010 or 60011 or
                6201 or 62010 or 62011 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Freezing Drizzle / Light Freezing Rain / Freezing Rain
                 */
                6003 or 60030 or 60031 or
                6002 or 60020 or 60021 or
                6004 or 60040 or 60041 or
                6204 or 62040 or 62041 or
                6206 or 62060 or 62061 or
                6205 or 62050 or 62051 or
                6203 or 62030 or 62031 or
                6209 or 62090 or 62091 or
                6213 or 62130 or 62131 or
                6214 or 62140 or 62141 or
                6215 or 62150 or 62151 or
                /*
                 * Mixed conditions:
                 * Condition 1: Drizzle / Light Rain / Rain
                 * Condition 2: Freezing Rain
                 */
                6212 or 62120 or 62121 or
                6222 or 62220 or 62221 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Heavy Freezing Rain
                 */
                6207 or 62070 or 62071 or
                6202 or 62020 or 62021 or
                6208 or 62080 or 62081 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_freezingrain"),
                /*
                 * Mixed conditions:
                 * Condition 1: Snow
                 * Condition 2: Ice Pellets
                 */
                5112 or 51120 or 51121 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_snowandsleet"),
                /* Light Ice Pellets / Ice Pellets / Heavy Ice Pellets / [Sleet] */
                7102 or 71020 or 71021 or
                7000 or 70000 or 70001 or
                7101 or 71010 or 71011 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Light Ice Pellets / Ice Pellets / Heavy Ice Pellets
                 */
                7110 or 71100 or 71101 or
                7111 or 71110 or 71111 or
                7112 or 71120 or 71121 or
                7108 or 71080 or 71081 or
                7107 or 71070 or 71071 or
                7109 or 71090 or 71091 or
                7113 or 71130 or 71131 or
                7114 or 71140 or 71141 or
                7116 or 71160 or 71161 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_sleet"),
                /*
                 * Mixed conditions:
                 * Condition 1: Drizzle / Light Rain / Rain / Freezing Rain
                 * Condition 2: Light Ice Pellets / Ice Pellets / Heavy Ice Pellets
                 */
                7105 or 71050 or 71051 or
                7115 or 71150 or 71151 or
                7117 or 71170 or 71171 or
                7106 or 71060 or 71061 or
                7103 or 71030 or 71031 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_rainandsleet"),
                /* Thunderstorm */
                8000 or 80000 or 80001 or
                /*
                 * Mixed conditions:
                 * Condition 1: Mostly Clear / Partly Cloudy / Mostly Cloudy
                 * Condition 2: Thunderstorm
                 */
                8001 or 80010 or 80011 or
                8003 or 80030 or 80031 or
                8002 or 80020 or 80021 => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_tstorms"),
                _ => SharedModule.Instance.ResLoader.GetString("/WeatherConditions/weather_notavailable"),
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