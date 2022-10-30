using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web;

namespace SimpleWeather.AccuWeather
{
    public partial class AccuWeatherProvider : WeatherProviderImpl
    {
        private const string DAILY_5DAY_FORECAST_URL = "https://dataservice.accuweather.com/forecasts/v1/daily/5day";
        private const string HOURLY_12HR_FORECAST_URL = "https://dataservice.accuweather.com/forecasts/v1/hourly/12hour";
        private const string CURRENT_CONDITIONS_URL = "https://dataservice.accuweather.com/currentconditions/v1";

        public AccuWeatherProvider() : base()
        {
            LocationProvider = new AccuWeatherLocationProvider();
        }

        public override string WeatherAPI => WeatherData.WeatherAPI.AccuWeather;
        public override bool SupportsWeatherLocale => true;
        public override bool KeyRequired => true;
        public override bool NeedsExternalAlertData => true;
        public override AuthType AuthType => AuthType.ApiKey;

        public override long GetRetryTime()
        {
            return 43200000L; // 12 hrs
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

                var requestUri = CURRENT_CONDITIONS_URL.ToUriBuilderEx()
                    .AppendQueryParameter("apikey", key)
                    .BuildUri();

                // Connect to webstream
                var webClient = SharedModule.Instance.WebClient;
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                using (var response = await webClient.GetAsync(requestUri, cts.Token))
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
                        case HttpStatusCode.NotFound:
                            isValid = ((await response.Content.ReadAsStringAsync())?.Length ?? 0) > 0;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                isValid = false;

                if (WebError.GetStatus(ex.HResult) > WebErrorStatus.Unknown || ex is HttpRequestException || ex is SocketException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NetworkError, ex);
                }
                else if (ex is WeatherException)
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
            return APIKeys.GetAccuWeatherKey();
        }

        /// <exception cref="WeatherException">Thrown when task is unable to retrieve data</exception>
        public override async Task<Weather> GetWeather(string location_query, string country_code)
        {
            Weather weather = null;
            WeatherException wEx = null;

            var culture = CultureUtils.UserCulture;
            string locale = LocaleToLangCode(culture.TwoLetterISOLanguageName, culture.Name);

            var key = Settings.UsePersonalKey ? Settings.APIKeys[WeatherData.WeatherAPI.AccuWeather] : GetAPIKey();

            if (String.IsNullOrWhiteSpace(key))
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.InvalidAPIKey);
            }

            try
            {
                this.CheckRateLimit();

                DailyForecastRootobject dailyRoot;
                HourlyForecastRootobject hourlyRoot;
                CurrentRootobject currentRoot;

                var requestCurrentUri = CURRENT_CONDITIONS_URL.ToUriBuilderEx()
                    .AppendPath(location_query)
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("language", locale)
                    .AppendQueryParameter("details", "true")
                    .BuildUri();

                var request5dayUri = DAILY_5DAY_FORECAST_URL.ToUriBuilderEx()
                    .AppendPath(location_query)
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("language", locale)
                    .AppendQueryParameter("details", "true")
                    .AppendQueryParameter("metric", "true")
                    .BuildUri();

                var requestHourlyUri = HOURLY_12HR_FORECAST_URL.ToUriBuilderEx()
                    .AppendPath(location_query)
                    .AppendQueryParameter("apikey", key)
                    .AppendQueryParameter("language", locale)
                    .AppendQueryParameter("details", "true")
                    .AppendQueryParameter("metric", "true")
                    .BuildUri();

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestCurrentUri))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromMinutes(30));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(request, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    currentRoot = new CurrentRootobject()
                    {
                        Items = await JSONParser.DeserializerAsync<CurrentsItem[]>(stream)
                    };
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, request5dayUri))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(3));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(request, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    dailyRoot = await JSONParser.DeserializerAsync<DailyForecastRootobject>(stream);
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, requestHourlyUri))
                {
                    request.CacheRequestIfNeeded(KeyRequired, TimeSpan.FromHours(3));

                    // Get response
                    var webClient = SharedModule.Instance.WebClient;
                    using var cts = new CancellationTokenSource(Settings.READ_TIMEOUT);
                    using var response = await webClient.SendAsync(request, cts.Token);

                    await this.CheckForErrors(response);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();

                    // Load weather
                    hourlyRoot = new HourlyForecastRootobject()
                    {
                        Items = await JSONParser.DeserializerAsync<HourlyItem[]>(stream)
                    };
                }

                weather = new Weather(dailyRoot, hourlyRoot, currentRoot);
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

                Logger.WriteLine(LoggerLevel.Error, ex, "AccuWeatherProvider: error getting weather data");
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

        protected override Task UpdateWeatherData(LocationData location, WeatherData.Weather weather)
        {
            // no-op
            return Task.CompletedTask;
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        public override string UpdateLocationQuery(WeatherData.Weather weather)
        {
            // TODO: make a task?
            var locationModel = LocationProvider.GetLocation(new WeatherUtils.Coordinate(weather.location.latitude.Value, weather.location.longitude.Value), WeatherAPI).Result;

            return locationModel.Location_Query;
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        public override string UpdateLocationQuery(LocationData location)
        {
            // TODO: make a task?
            var locationModel = LocationProvider.GetLocation(new WeatherUtils.Coordinate(location.latitude, location.longitude), WeatherAPI).Result;

            return locationModel.Location_Query;
        }

        public override string LocaleToLangCode(string iso, string name)
        {
            return name;
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
                /* Sunny */
                1 => WeatherIcons.DAY_SUNNY,
                /*
                 *  2: Mostly Sunny
                 *  3: Partly Sunny
                 *  4: Intermittent Clouds
                 */
                2 or 3 or 4 => WeatherIcons.DAY_PARTLY_CLOUDY,
                /* 5: Hazy Sunshine */
                5 => WeatherIcons.DAY_HAZE,
                /* 6: Mostly Cloudy */
                6 => WeatherIcons.DAY_CLOUDY,
                /* 7: Cloudy */
                7 => WeatherIcons.CLOUDY,
                /* 8: Dreary (Overcast) */
                8 => WeatherIcons.OVERCAST,
                /* 11: Fog */
                11 => WeatherIcons.FOG,
                /* 12: Showers */
                12 => WeatherIcons.SHOWERS,
                /*
                 * 13: Mostly Cloudy w/ Showers
                 * 14: Partly Sunny w/ Showers
                 */
                13 or 14 => WeatherIcons.DAY_SHOWERS,
                /* 15: T-Storms */
                15 => WeatherIcons.THUNDERSTORM,
                /*
                 * 16: Mostly Cloudy w/ T-Storms
                 * 17: Partly Sunny w/ T-Storms
                 */
                16 or 17 => WeatherIcons.DAY_THUNDERSTORM,
                /* 18: Rain */
                18 => WeatherIcons.RAIN,
                /* 19: Flurries */
                19 => WeatherIcons.SNOW,
                /*
                 * 20: Mostly Cloudy w/ Flurries
                 * 21: Partly Sunny w/ Flurries
                 * 23: Mostly Cloudy w/ Snow
                 */
                20 or 21 or 23 => WeatherIcons.DAY_SNOW,
                /* 22: Snow */
                22 => WeatherIcons.SNOW,
                /*
                 * 24: Ice
                 * 31: Cold
                 */
                24 or 31 => WeatherIcons.SNOWFLAKE_COLD,
                /* 25: Sleet */
                25 => WeatherIcons.SLEET,
                /*
                 * 26: Freezing Rain
                 * 29: Rain and Snow
                 */
                26 or 29 => WeatherIcons.RAIN_MIX,
                /* 30: Hot */
                30 => isNight ? WeatherIcons.NIGHT_HOT : WeatherIcons.DAY_HOT,
                /* 32: Windy */
                32 => WeatherIcons.WINDY,
                /* 33: Clear */
                33 => WeatherIcons.NIGHT_CLEAR,
                /*
                 * 34: Mostly Clear
                 * 35: Partly Cloudy
                 * 36: Intermittent Clouds
                 */
                34 or 35 or 36 => WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY,
                /* 37: Hazy Moonlight */
                37 => WeatherIcons.NIGHT_HAZE,
                /* 38: Mostly Cloudy */
                38 => WeatherIcons.NIGHT_ALT_CLOUDY,
                /*
                 * 39: Partly Cloudy w/ Showers
                 * 40: Mostly Cloudy w/ Showers
                 */
                39 or 40 => WeatherIcons.NIGHT_ALT_SHOWERS,
                /*
                 * 41: Partly Cloudy w/ T-Storms
                 * 42: Mostly Cloudy w/ T-Storms
                 */
                41 or 42 => WeatherIcons.NIGHT_ALT_THUNDERSTORM,
                /*
                 * 43: Mostly Cloudy w/ Flurries
                 * 44: Mostly Cloudy w/ Snow
                 */
                43 or 44 => WeatherIcons.NIGHT_ALT_SNOW,

                _ => String.Empty,
            };

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                this.LogMissingIcon(icon);
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }
    }
}