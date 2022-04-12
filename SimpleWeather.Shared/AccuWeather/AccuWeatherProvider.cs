using SimpleWeather.Icons;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.SMC;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.Web;
using System.Net.Http;
using System.Net.Sockets;
using System.Net;
using System.Net.Http.Headers;

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
                var webClient = SimpleLibrary.GetInstance().WebClient;
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
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(30)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
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
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromHours(3)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
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
                    request.Headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromHours(3)
                    };

                    // Get response
                    var webClient = SimpleLibrary.GetInstance().WebClient;
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

            return locationModel.LocationQuery;
        }

        /// <exception cref="WeatherException">Ignore.</exception>
        public override string UpdateLocationQuery(LocationData location)
        {
            // TODO: make a task?
            var locationModel = LocationProvider.GetLocation(new WeatherUtils.Coordinate(location.latitude, location.longitude), WeatherAPI).Result;

            return locationModel.LocationQuery;
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
            string WeatherIcon = string.Empty;

            if (icon == null || !int.TryParse(icon, out int conditionCode))
                return WeatherIcons.NA;

            switch (conditionCode)
            {
                /*
                 *  1: Sunny
                 * 33: Clear
                 */
                case 1:
                case 33:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_SUNNY;
                    break;

                /*
                 *  2: Mostly Sunny
                 *  3: Partly Sunny
                 * 34: Mostly Clear
                 * 35: Partly Cloudy
                 */
                case 2:
                case 3:
                case 34:
                case 35:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY : WeatherIcons.DAY_PARTLY_CLOUDY;
                    break;

                /*
                 *  4: Intermittent Clouds
                 *  6: Mostly Cloudy
                 *  7: Cloudy
                 * 36: Intermittent Clouds
                 * 38: Mostly Cloudy
                 */
                case 4:
                case 6:
                case 7:
                case 36:
                case 38:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_CLOUDY : WeatherIcons.DAY_CLOUDY;
                    break;

                /* 8: Dreary (Overcast) */
                case 8:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_OVERCAST : WeatherIcons.DAY_SUNNY_OVERCAST;
                    break;

                /*
                 *  5: Hazy Sunshine
                 * 37: Hazy Moonlight
                 */
                case 5:
                case 37:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_HAZE;
                    break;

                /* 11: Fog */
                case 11:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_FOG : WeatherIcons.DAY_FOG;
                    break;

                /*
                 * 12: Showers
                 * 13: Mostly Cloudy w/ Showers
                 * 14: Partly Sunny w/ Showers
                 * 39: Partly Cloudy w/ Showers
                 * 40: Mostly Cloudy w/ Showers
                 */
                case 12:
                case 13:
                case 14:
                case 39:
                case 40:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SHOWERS : WeatherIcons.DAY_SHOWERS;
                    break;

                /*
                 * 15: T-Storms
                 * 16: Mostly Cloudy w/ T-Storms
                 * 17: Partly Sunny w/ T-Storms
                 * 41: Partly Cloudy w/ T-Storms
                 * 42: Mostly Cloudy w/ T-Storms
                 */
                case 15:
                case 16:
                case 17:
                case 41:
                case 42:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_THUNDERSTORM : WeatherIcons.DAY_THUNDERSTORM;
                    break;

                /* 18: Rain */
                case 18:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN : WeatherIcons.DAY_RAIN;
                    break;

                /*
                 * 19: Flurries
                 * 20: Mostly Cloudy w/ Flurries
                 * 21: Partly Sunny w/ Flurries
                 * 22: Snow
                 * 23: Mostly Cloudy w/ Snow
                 * 43: Mostly Cloudy w/ Flurries
                 * 44: Mostly Cloudy w/ Snow
                 */
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 43:
                case 44:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SNOW : WeatherIcons.DAY_SNOW;
                    break;

                /*
                 * 24: Ice
                 * 31: Cold
                 */
                case 24:
                case 31:
                    WeatherIcon = WeatherIcons.SNOWFLAKE_COLD;
                    break;

                /* 25: Sleet */
                case 25:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_SLEET : WeatherIcons.DAY_SLEET;
                    break;

                /*
                 * 26: Freezing Rain
                 * 29: Rain and Snow
                 */
                case 26:
                case 29:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_ALT_RAIN_MIX : WeatherIcons.DAY_RAIN_MIX;
                    break;

                /* 30: Hot */
                case 30:
                    WeatherIcon = isNight ? WeatherIcons.NIGHT_CLEAR : WeatherIcons.DAY_HOT;
                    break;

                /* 32: Windy */
                case 32:
                    WeatherIcon = isNight ? WeatherIcons.WINDY : WeatherIcons.DAY_WINDY;
                    break;
            }

            if (String.IsNullOrWhiteSpace(WeatherIcon))
            {
                // Not Available
                WeatherIcon = WeatherIcons.NA;
            }

            return WeatherIcon;
        }
    }
}